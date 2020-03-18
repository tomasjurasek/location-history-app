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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services;

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
        private readonly IHttpClientFactory httpClientFactory;

        public UsersController(UserLocationsService locationService,
            ILogger<UsersController> logger,
            IConfiguration config,
            AzureBlobService azureBlobService,
            LocationCreatedSender locationCreatedSender,
            IHttpClientFactory httpClientFactory
            )
        {
            this.locationService = locationService;
            this.logger = logger;
            this.config = config;
            this.azureBlobService = azureBlobService;
            this.locationCreatedSender = locationCreatedSender;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("{userId}/locations")]
        public async Task<ActionResult<IList<LocationViewModel>>> Get(string userId)
        {
            var keboolaClient = httpClientFactory.CreateClient("Keboola");
            var requesResponse = await keboolaClient.GetAsync($"v2/storage/tables/in.c-keboola-ex-aws-s3-117966293.data/data-preview?format=json&whereFilters[0][column]=id&whereFilters[0][operator]=eq&whereFilters[0][values][0]={userId}");
            var json = await requesResponse.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<KebolaDataRoot>(json);

            var locations = new List<LocationViewModel>();
            for (int i = 0; i <= data.Rows.Length - 1; i++)
            {
                var location = new LocationViewModel();
                for (int y = 0; y <= data.Rows[i].Length - 1; y++)
                {
                    var row = data.Rows[i][y];
                    if (row.ColumnName == "date")
                    {
                        location.DateTimeUtc = Convert.ToDateTime(row.Value);
                    }
                    else if (row.ColumnName == "accuracy")
                    {
                        location.Accuracy = int.Parse(row.Value);
                    }
                    else if (row.ColumnName == "longitude")
                    {
                        location.Longitude = int.Parse(row.Value);
                    }
                    else if (row.ColumnName == "latitude")
                    {
                        location.Latitude = int.Parse(row.Value);
                    }
                }

                locations.Add(location);
            }
           
            return locations;
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