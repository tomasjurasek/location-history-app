using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Services
{
    public class GoogleLocationParser
    {
        public IEnumerable<Locations> Parse(byte[] data)
        {
            var response = new List<Locations>();
            GoogleRootObject jsonData;

            using (var reader = new StreamReader(new MemoryStream(data), Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
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