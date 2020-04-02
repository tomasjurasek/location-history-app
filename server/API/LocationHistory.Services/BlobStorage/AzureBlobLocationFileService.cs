using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using LocationHistory.Services.Options;
using Microsoft.Extensions.Options;

namespace LocationHistory.Services.BlobStorage
{
    public class AzureBlobLocationFileService
    {
        private BlobContainerClient containerClient;

        public AzureBlobLocationFileService(IOptions<AzureBlobServiceOptions> options)
        {
            var storageConnectionString = options.Value.StorageAccount;
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient("locationfile");
        }

        public async Task Upload(string userId, Stream stream)
        {
            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.zip");
            await blobClient.UploadAsync(stream);
        }

        public async Task<Stream> Download(string userId)
        {
            var stream = new MemoryStream();
            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.zip");
            await blobClient.DownloadToAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task Delete(string userId)
        {
            await containerClient.DeleteBlobIfExistsAsync($"{userId}.zip");
        }
    }
}
