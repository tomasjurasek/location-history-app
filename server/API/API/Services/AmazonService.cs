﻿using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{

    public class AmazonService
    {
        private readonly IOptions<AmazonOptions> amazonOptions;

        public AmazonService(IOptions<AmazonOptions> amazonOptions)
        {
            this.amazonOptions = amazonOptions;
        }

        public async Task UploadCsvData(string userId, IEnumerable<Locations> locations)
        {
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
