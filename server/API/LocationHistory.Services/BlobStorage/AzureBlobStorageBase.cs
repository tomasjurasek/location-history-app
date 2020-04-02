using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationHistory.Services.BlobStorage
{
    public abstract class AzureBlobStorageBase
    {
        protected BlobContainerClient containerClient;

        public AzureBlobStorageBase(string storageConnectionString, string queue)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient(queue);
        }
    }
}
