using Amazon.Runtime.Internal.Util;
using Azure.Storage.Blobs;
using LocationHistory.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LocationHistory.Services.BlobStorage
{
    public class AzureBlobLocationDataFileService : AzureBlobStorageBase
    {
        private readonly ILogger<AzureBlobLocationDataFileService> logger;
        private const string QUEUE_NAME = "locationdatafile";

        public AzureBlobLocationDataFileService(IOptions<AzureBlobServiceOptions> options, ILogger<AzureBlobLocationDataFileService> logger) : base(options.Value.StorageAccount, QUEUE_NAME)
        {
            this.logger = logger;
        }

        public async Task UploadCsvData(string userId, Stream data)
        {
            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.csv");
            await blobClient.UploadAsync(data);
        }

        public async Task<Stream> Download(string userId)
        {
            var stream = new MemoryStream();
            try
            {
                BlobClient blobClient = containerClient.GetBlobClient($"{userId}.csv");
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Download CSV file failed");
            }
          
            return stream;
        }

        public async Task<bool> Delete(string userId)
        {
            return await containerClient.DeleteBlobIfExistsAsync($"{userId}.csv");
        }

       
    }
}
