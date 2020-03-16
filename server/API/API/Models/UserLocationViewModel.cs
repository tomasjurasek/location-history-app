using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class UserLocationViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("locations")]
        public ICollection<LocationViewModel> Locations { get; set; }
    }
}
