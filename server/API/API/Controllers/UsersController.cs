using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
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
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration config;

        public UsersController(UserLocationsService locationService, ILogger<UsersController> logger, IWebHostEnvironment env, IConfiguration config)
        {
            this.locationService = locationService;
            this.logger = logger;
            this.env = env;
            this.config = config;
        }

        [HttpPost("{userId}/file")]
        [RequestSizeLimit(104857600)]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult<string>> UploadFileAsync(string userId, [FromForm] IFormFile file)
        {
            string tempDirectoryPath = null;

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
                tempDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"{userId}");
                Directory.CreateDirectory(tempDirectoryPath);

                var uploadedFilePath = Path.Combine(tempDirectoryPath, file.FileName);
                await using (Stream stream = new FileStream(uploadedFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Processing of uploaded file failed.");
            }

            return userId;
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