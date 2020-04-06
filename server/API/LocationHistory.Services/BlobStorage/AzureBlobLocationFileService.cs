using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Azure.Storage.Blobs;
using LocationHistory.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LocationHistory.Services.BlobStorage
{
    public class AzureBlobLocationFileService : AzureBlobStorageBase
    {
        private readonly ILogger<AzureBlobLocationFileService> logger;
        private const string QUEUE_NAME = "locationfile";

        public AzureBlobLocationFileService(IOptions<AzureBlobServiceOptions> options, ILogger<AzureBlobLocationFileService> logger) : base(options.Value.StorageAccount, QUEUE_NAME)
        {
            this.logger = logger;
        }

        public async Task Upload(string userId, Stream stream)
        {
            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.zip");
            await blobClient.UploadAsync(stream);
        }

        public async Task<Stream> Download(string userId)
        {
            var stream = new MemoryStream();
            try
            {
                BlobClient blobClient = containerClient.GetBlobClient($"{userId}.zip");
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
               
            }
            catch (System.Exception ex)
            {
                logger.LogWarning(ex, "Download CSV file failed");
            }

            return stream;
        }

        public async Task<bool> Delete(string userId)
        {
            return await containerClient.DeleteBlobIfExistsAsync($"{userId}.zip");
        }
    }
}
