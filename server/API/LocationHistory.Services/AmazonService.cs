﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using LocationHistory.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LocationHistory.Services
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

        public async Task UploadCsvData(string userId, string phone, IEnumerable<Locations> locations)
        {
            try
            {
                logger.LogInformation("Start: Upload csv for {UserId}", userId);
                using (var client = new AmazonS3Client(amazonOptions.Value.Key, amazonOptions.Value.Secret, RegionEndpoint.EUCentral1))
                {
                    var csvData = ConvertToCsv(phone, locations);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = GenerateStream(csvData),
                        Key = $"{userId}.csv",
                        BucketName = amazonOptions.Value.Bucket,
                        CannedACL = S3CannedACL.Private
                    };

                    logger.LogInformation("Uploading data into Amazaon S3 for {UserId}", userId);
                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                logger.LogInformation("Finish: Upload csv for {UserId}", userId);
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
                logger.LogInformation("Start: Get locations from Amazon S3 for {UserId}", userId);

                using (var client = new AmazonS3Client(amazonOptions.Value.Key, amazonOptions.Value.Secret, RegionEndpoint.EUCentral1))
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), $"amazon-{userId}");
                    Directory.CreateDirectory(folder);
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

                logger.LogInformation("Finish: Get locations from Amazon S3 for {UserId}", userId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(UploadCsvData));
            }

            return response;
        }

        public async Task<bool> Delete(string userId)
        {
            try
            {
                using (var client = new AmazonS3Client(amazonOptions.Value.Key, amazonOptions.Value.Secret, RegionEndpoint.EUCentral1))
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), $"amazon-delete-{userId}");
                    Directory.CreateDirectory(folder);
                    var file = Path.Combine(folder, $"{userId}.csv");

                    var headerFile = "id,date,longitude,latitude,accuracy";

                    using (var fileStream = File.Create(file))
                    {
                        using (var stream = GenerateStream(headerFile))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(file, amazonOptions.Value.Bucket);

                    Directory.Delete(folder, true);

                    return true;
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(UploadCsvData));
            }

            return false;
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



        private static string ConvertToCsv(string phone, IEnumerable<Locations> locations)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,date,longitude,latitude,accuracy,createdDateUtc");

            foreach (var location in locations)
            {
                stringBuilder.AppendLine($"{phone},{location.DateTimeUtc},{location.Longitude},{location.Latitude},{location.Accuracy},{DateTime.UtcNow}");
            }
            return stringBuilder.ToString();
        }

        private static MemoryStream GenerateStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
