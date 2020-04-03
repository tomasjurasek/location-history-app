using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using LocationHistory.API.Extensions;
using LocationHistory.API.Models;
using LocationHistory.Database;
using LocationHistory.Services;
using LocationHistory.Services.BlobStorage;
using LocationHistory.Services.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LocationHistory.API.Controllers
{
    [Route("api/users")]
    public partial class UsersController : Controller
    {
        private readonly ILogger<UsersController> logger;
        private readonly IConfiguration config;
        private readonly AzureBlobLocationFileService azureBlobService;
        private readonly AzureBlobLocationDataFileService azureBlobLocationDataFileService;
        private readonly LocationCreatedSender locationCreatedSender;
        private readonly LocationDbContext locationDbContext;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly UserLocationsService userLocationsService;

        public static ConcurrentDictionary<string, int> userSmSTreshhold { get; set; } = new ConcurrentDictionary<string, int>();

        public UsersController(ILogger<UsersController> logger,
            IConfiguration config,
            AzureBlobLocationFileService azureBlobService,
            AzureBlobLocationDataFileService azureBlobLocationDataFileService,
            LocationCreatedSender locationCreatedSender,
            LocationDbContext locationDbContext,
            IHttpClientFactory httpClientFactory,
            UserLocationsService userLocationsService)
        {
            this.logger = logger;
            this.config = config;
            this.azureBlobService = azureBlobService;
            this.azureBlobLocationDataFileService = azureBlobLocationDataFileService;
            this.locationCreatedSender = locationCreatedSender;
            this.locationDbContext = locationDbContext;
            this.httpClientFactory = httpClientFactory;
            this.userLocationsService = userLocationsService;
        }

        [HttpGet("{userId}/verify")]
        public async Task<ActionResult> Verify(string userId, [FromQuery]string verifyCode)
        {
            var user = await locationDbContext.Users.FirstOrDefaultAsync(s => s.UserIdentifier == userId && s.VerifyCode == verifyCode.ToUpper());
            if (user != null)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpPost("send")]
        public async Task<ActionResult> Send([FromBody]string phoneNumber)
        {

            userSmSTreshhold.TryGetValue(phoneNumber, out var smsCount);

            if (smsCount >= 3 || !phoneNumber.IsValidPhone())
            {
                return BadRequest();
            }

            var userId = $"{RandomString(3)}-{RandomString(3)}-{RandomString(3)}";
            var token = Guid.NewGuid();
            var verifyCode = RandomString(5);

            var client = httpClientFactory.CreateClient();

            var smsToken = config.GetValue<string>("SmsToken");
            var smsUrl = config.GetValue<string>("SmsUrl");
            var encodedText = HttpUtility.UrlEncode($"(Poloha pro hygienu) potvrzovaci kod: {verifyCode}");
            var url = string.Format(smsUrl, smsToken, phoneNumber, encodedText);

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                locationDbContext.Users.Add(new Database.Entities.User
                {
                    Status = Database.Entities.Status.InProgress,
                    Token = token.ToString(),
                    Phone = phoneNumber.GetPhone(),
                    UserIdentifier = userId,
                    VerifyCode = verifyCode,
                    CreatedDateTime = DateTime.UtcNow
                });

                await locationDbContext.SaveChangesAsync();
                userSmSTreshhold.AddOrUpdate(phoneNumber, 1, (key, count) => count + 1);

                return Ok(userId);
            }

            return BadRequest();

        }
        [HttpGet("{userId}/locations")]
        public async Task<ActionResult<IList<LocationViewModel>>> Get(string userId, [FromQuery]string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing.");
            }

            var user = await locationDbContext.Users.FirstOrDefaultAsync(s => s.UserIdentifier == userId && s.Token == token);

            if (user != null)
            {
                var locations = new List<LocationViewModel>();
                var data = await userLocationsService.GetLocations(userId);

                locations.AddRange(data.Select(s => new LocationViewModel
                {
                    DateTimeUtc = s.DateTimeUtc,
                    Accuracy = s.Accuracy,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                }));

                if (locations.Any())
                {
                    return locations;
                }

                if (user.Status == Database.Entities.Status.InProgress)
                {
                    return NoContent();
                }
            }

            return NotFound();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(string userId, [FromQuery]string token)
        {
            var user = await locationDbContext.Users.FirstOrDefaultAsync(s => s.UserIdentifier == userId && s.Token == token);
            if (user != null)
            {
                await userLocationsService.DeleteUserData(userId);

                locationDbContext.Users.Remove(user);
                await locationDbContext.SaveChangesAsync();

                return Ok();
            }

            return NotFound();
        }

        [HttpPost("{userId}/file")]
        [RequestSizeLimit(104857600)]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult<UserLocationViewModel>> UploadFileAsync([FromForm] IFormFile file, string userId, [FromQuery]string verifyCode)
        {

            var user = await locationDbContext.Users.FirstOrDefaultAsync(s => s.UserIdentifier == userId && s.VerifyCode == verifyCode.ToUpper());

            if (user != null)
            {
                var token = Guid.NewGuid();
                var response = new UserLocationViewModel
                {
                    Id = userId,
                    Token = user.Token
                };

                if (file == null)
                {
                    return BadRequest("No file has been uploaded.");
                }

                if (!IsFileLengthValid(file))
                {
                    return BadRequest("File size exceeded configured limit.");
                }

                try
                {
                    using (Stream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;
                        await azureBlobService.Upload(userId, stream);
                        await locationCreatedSender.SendMessageAsync(new LocationsCreatedMessage
                        {
                            UserId = userId
                        });
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Processing of uploaded file failed.", ex);
                }

                return response;

            }

            return NotFound();

        }

        private static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFHJKLMNPRSTUVXYZ2345789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool IsFileLengthValid(IFormFile file)
        {
            var maxMaxUploadedFileLength = config.GetValue<long>("MaxUploadedFileLength");
            if (maxMaxUploadedFileLength == default)
            {
                throw new Exception("Missing configuration of 'MaxUploadedFileLength'.");
            }

            return file.Length <= maxMaxUploadedFileLength;
        }
    }
}