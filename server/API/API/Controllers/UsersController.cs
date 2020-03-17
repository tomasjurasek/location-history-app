using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.ServiceBus;
using API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/users")]
    public partial class UsersController : Controller
    {
        private readonly UserLocationsService locationService;
        private readonly ILogger<UsersController> logger;
        private readonly IConfiguration config;
        private readonly AzureBlobService azureBlobService;
        private readonly LocationCreatedSender locationCreatedSender;

        public UsersController(UserLocationsService locationService,
            ILogger<UsersController> logger,
            IConfiguration config,
            AzureBlobService azureBlobService,
            LocationCreatedSender locationCreatedSender
            )
        {
            this.locationService = locationService;
            this.logger = logger;
            this.config = config;
            this.azureBlobService = azureBlobService;
            this.locationCreatedSender = locationCreatedSender;
        }

        [HttpPost("file")]
        [RequestSizeLimit(104857600)]
        public async Task<ActionResult<UserLocationViewModel>> UploadFileAsync([FromForm] IFormFile file)
        {
            var userId = Guid.NewGuid().ToString();
            var response = new UserLocationViewModel
            {
                Id = userId
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