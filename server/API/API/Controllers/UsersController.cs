using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Runtime;
using API.Models;
using API.Services;
using API.Services.ServiceBus;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services;

namespace API.Controllers
{
    [Route("api/users")]
    public partial class UsersController : Controller
    {
        private readonly ILogger<UsersController> logger;
        private readonly IConfiguration config;
        private readonly AzureBlobService azureBlobService;
        private readonly LocationCreatedSender locationCreatedSender;
        private readonly LocationDbContext locationDbContext;
        private readonly AmazonService amazonService;

        public UsersController(ILogger<UsersController> logger,
            IConfiguration config,
            AzureBlobService azureBlobService,
            LocationCreatedSender locationCreatedSender,
            LocationDbContext locationDbContext,
            AmazonService amazonService
            )
        {
            this.logger = logger;
            this.config = config;
            this.azureBlobService = azureBlobService;
            this.locationCreatedSender = locationCreatedSender;
            this.locationDbContext = locationDbContext;
            this.amazonService = amazonService;
        }


        [HttpGet("send")]
        public async Task<ActionResult> Send([FromQuery]string phoneNumber)
        {
            var userId = $"{RandomString(3)}-{RandomString(3)}-{RandomString(3)}";
            var token = Guid.NewGuid();
            var client = new HttpClient();
            var verifyCode = RandomString(5);
            var smsToken = config.GetValue<string>("SmsToken");
            var smsUrl = config.GetValue<string>("SmsUrl");
            var url = string.Format(smsUrl, smsToken, phoneNumber, verifyCode);
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                locationDbContext.Users.Add(new Database.Entities.User
                {
                    Status = Database.Entities.Status.InProgress,
                    Token = token.ToString(),
                    Phone = phoneNumber,
                    UserIdentifier = userId,
                    VerifyCode = verifyCode,
                    CreatedDateTime = DateTime.UtcNow
                });

                await locationDbContext.SaveChangesAsync();

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
                var data = await amazonService.GetLocations(userId);

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
                var result = await amazonService.Delete(userId);
                if (result)
                {
                    return Ok();
                }
                return BadRequest();
            }

            return NotFound();
        }

        public class KebolaDataRoot
        {
            [JsonProperty("rows")]
            public KebolaDataRow[][] Rows { get; set; }
            [JsonProperty("columns")]
            public string[] Columns { get; set; }
        }

        public class KebolaDataRow
        {
            [JsonProperty("columnName")]
            public string ColumnName { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("isTruncated")]
            public bool IsTruncated { get; set; }
        }

        [HttpPost("file")]
        [RequestSizeLimit(104857600)]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult<UserLocationViewModel>> UploadFileAsync([FromForm] IFormFile file, [FromQuery]string userId, [FromQuery]string verifyCode)
        {

            var user = await locationDbContext.Users.FirstOrDefaultAsync(s => s.UserIdentifier == userId && s.VerifyCode == verifyCode);

            if (user != null)
            {
                var token = Guid.NewGuid();
                var response = new UserLocationViewModel
                {
                    Id = userId,
                    Token = token.ToString()
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
                        await azureBlobService.UploadFile(userId, stream);
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
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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