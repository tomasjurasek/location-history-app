using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
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

        public async Task UploadCsvData(string userId, string csvData)
        {
            using (var client = new AmazonS3Client(amazonOptions.Value.Key, amazonOptions.Value.Secret, RegionEndpoint.USEast1))
            {
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

        private static MemoryStream GenerateStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
