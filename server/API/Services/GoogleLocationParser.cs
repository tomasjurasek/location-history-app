using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Services
{
    public class GoogleLocationParser
    {
        public IEnumerable<Locations> Parse(string jsonFilePath)
        {
            var response = new List<Locations>();
            GoogleRootObject jsonData;

            using (StreamReader file = File.OpenText(jsonFilePath))
            {
                var serializer = new JsonSerializer();
                jsonData = (GoogleRootObject)serializer.Deserialize(file, typeof(GoogleRootObject));
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

            return response
                .Where(s => s.DateTimeUtc >= new DateTime(2020, 3, 1))
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