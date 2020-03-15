using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class LocationViewModel
    {
        [JsonProperty("dateTime")]
        public DateTime DateTimeUtc { get; set; }
        [JsonProperty("longitude")]
        public int Longitude { get; set; }
        [JsonProperty("latitude")]
        public int Latitude { get; set; }
        [JsonProperty("accuracy")]
        public int? Accuracy { get; set; }
    }
}
