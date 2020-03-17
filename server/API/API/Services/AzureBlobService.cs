using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class AzureBlobService
    {
        private BlobContainerClient containerClient;

        public AzureBlobService(IConfiguration config)
        {
            var storageConnectionString = config.GetValue<string>("AzureStorageAccount");
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient("locationfile");
        }
        public async Task UploadFile(string userId, Stream stream)
        {
            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.zip");
            await blobClient.UploadAsync(stream);
        }

        public async Task<Stream> DownloadFile(string userId)
        {
            var stream = new MemoryStream();

            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.zip");
            await blobClient.DownloadToAsync(stream);

            return stream;

        }

        public async Task DeleteFile(string userId)
        {
            await containerClient.DeleteBlobIfExistsAsync($"{userId}.zip");
        }
    }
}
