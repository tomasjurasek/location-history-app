using Azure.Storage.Blobs;
using LocationHistory.Services.Options;
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
        public AzureBlobLocationDataFileService(IOptions<AzureBlobServiceOptions> options) : base(options.Value.StorageAccount, "locationdatafile")
        {
        }

        public async Task UploadCsvData(string userId, string phone, IEnumerable<Locations> locations)
        {
            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.csv");
            var csvData = ConvertToCsv(phone, locations);
            await blobClient.UploadAsync(GenerateStream(csvData));
        }

        public async Task<Stream> Download(string userId)
        {
            BlobClient blobClient = containerClient.GetBlobClient($"{userId}.csv");
            var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<bool> Delete(string userId)
        {
            return await containerClient.DeleteBlobIfExistsAsync($"{userId}.csv");
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

            var createdDateTimeUtc = DateTime.UtcNow.ToString("dd/MM/yyyy H:mm", CultureInfo.InvariantCulture);

            foreach (var location in locations)
            {
                stringBuilder.AppendLine($"{phone},{location.DateTimeUtc.ToString("dd/MM/yyyy H:mm", CultureInfo.InvariantCulture)},{location.Longitude},{location.Latitude},{location.Accuracy},{createdDateTimeUtc}");
            }
            return stringBuilder.ToString();
        }

        private static MemoryStream GenerateStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
