using System;

namespace LocationHistory.API.Models
{
    public class LocationViewModel
    {
        public DateTime DateTimeUtc { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public int? Accuracy { get; set; }
    }
}
