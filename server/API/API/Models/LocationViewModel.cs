using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class LocationViewModel
    {
        public DateTime DateTimeUtc { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public int? Accuracy { get; set; }
    }
}
