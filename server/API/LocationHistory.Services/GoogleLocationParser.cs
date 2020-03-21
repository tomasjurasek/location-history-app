using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LocationHistory.Services
{
    public class GoogleLocationParser
    {
        private readonly ILogger<GoogleLocationParser> logger;

        public GoogleLocationParser(ILogger<GoogleLocationParser> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Locations> Parse(byte[] data)
        {
            var response = new List<Locations>();
            GoogleRootObject jsonData;

            using (var reader = new StreamReader(new MemoryStream(data), Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                logger.LogTrace($"Deserializing data into {nameof(GoogleRootObject)}.");
                jsonData = (GoogleRootObject)serializer.Deserialize(reader, typeof(GoogleRootObject));
            }

            foreach (var item in jsonData.Locations)
            {
                response.Add(new Locations
                {
                    DateTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(item.timestampMs)).UtcDateTime,
                    Latitude = item.latitudeE7,
                    Longitude = item.longitudeE7,
                    Accuracy = item.accuracy
                });
            }

            var threeWeeksAgo = DateTime.UtcNow.AddDays(-21);

            logger.LogTrace($"Filtering locations (>= {threeWeeksAgo}).");
            return response
                .Where(s => s.DateTimeUtc >= threeWeeksAgo)
                .OrderBy(s => s.DateTimeUtc);
        }
    }

    public class GoogleRootObject
    {
        [JsonProperty("locations")] public List<GoogleLocation> Locations { get; set; }
    }

    public class GoogleLocation
    {
        public string timestampMs { get; set; }
        public int latitudeE7 { get; set; }
        public int longitudeE7 { get; set; }
        public int? accuracy { get; set; }
        public int? velocity { get; set; }
        public int? altitude { get; set; }
        public int? verticalAccuracy { get; set; }
        public int? heading { get; set; }
    }
}