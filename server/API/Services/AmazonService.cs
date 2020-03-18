using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{

    public class AmazonService
    {
        private readonly IOptions<AmazonOptions> amazonOptions;
        private readonly ILogger<AmazonService> logger;

        public AmazonService(IOptions<AmazonOptions> amazonOptions, ILogger<AmazonService> logger)
        {
            this.amazonOptions = amazonOptions;
            this.logger = logger;
        }

        public async Task UploadCsvData(string userId, IEnumerable<Locations> locations)
        {
            try
            {
                logger.LogInformation($"Start: Upload csv for ${userId}");
                using (var client = new AmazonS3Client(amazonOptions.Value.Key, amazonOptions.Value.Secret, RegionEndpoint.EUCentral1))
                {
                    var csvData = ConvertToCsv(userId, locations);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = GenerateStream(csvData),
                        Key = $"{userId}.csv",
                        BucketName = amazonOptions.Value.Bucket,
                        CannedACL = S3CannedACL.Private
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                logger.LogInformation($"Finish: Upload csv for ${userId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(UploadCsvData));
            }
        }

        public async Task<List<Locations>> GetLocations(string userId)
        {
            var response = new List<Locations>();
            try
            {
                using (var client = new AmazonS3Client(amazonOptions.Value.Key, amazonOptions.Value.Secret, RegionEndpoint.EUCentral1))
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), $"amazon-{userId}");
                    var file = Path.Combine(folder, $"{userId}.csv");
                    var uploadRequest = new TransferUtilityDownloadRequest
                    {
                        Key = $"{userId}.csv",
                        BucketName = amazonOptions.Value.Bucket,
                        FilePath = file
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.DownloadAsync(uploadRequest);

                    var fileData = File.ReadAllLines(file);
                    response = ConvertFromCsv(fileData);
                    Directory.Delete(folder, true);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(UploadCsvData));
            }

            return response;
        }

        public static List<Locations> ConvertFromCsv(string[] fileData)
        {
            var locations = new List<Locations>();
            foreach (var csvLine in fileData)
            {
                string[] values = csvLine.Split(',');
                if (values[0] != "id")
                {
                    locations.Add(new Locations
                    {
                        DateTimeUtc = Convert.ToDateTime(values[1]),
                        Longitude = int.Parse(values[2]),
                        Latitude = int.Parse(values[3]),
                        Accuracy = int.Parse(values[4])
                    });

                }
            }

            return locations;
        }



        private static string ConvertToCsv(string userId, IEnumerable<Locations> locations)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,date,longitude,latitude,accuracy");

            foreach (var location in locations)
            {
                stringBuilder.AppendLine($"{userId},{location.DateTimeUtc},{location.Longitude},{location.Latitude},{location.Accuracy}");
            }
            return stringBuilder.ToString();
        }

        private static MemoryStream GenerateStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
