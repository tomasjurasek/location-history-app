using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static API.Controllers.UsersController;

namespace API.Services
{
    public class GoogleLocationParser
    {
        public IEnumerable<Locations> Parse(string json)
        {
            var response = new List<Locations>();
            var jsonData = JsonConvert.DeserializeObject<GoogleRootObject>(json);

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

        public string ParseToCsv(string userId, string json)
        {
            var jsonData = JsonConvert.DeserializeObject<GoogleRootObject>(json);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"id,date,longitude,latitude,accuracy");

            foreach (var item in jsonData.Locations)
            {
                var date = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(item.timestampMs)).UtcDateTime;
                if(date >= new DateTime(2020, 3, 1) 
                    && date <= new DateTime(2020, 5, 1))
                {
                    stringBuilder.AppendLine($"{userId},{date},{item.longitudeE7},{item.latitudeE7},{item.accuracy}");
                }
            }

            return stringBuilder.ToString();
        }
    }


    public class GoogleRootObject
    {
        [JsonProperty("locations")]
        public List<GoogleLocation> Locations { get; set; }
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
