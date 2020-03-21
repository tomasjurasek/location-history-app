using System;

namespace LocationHistory.Services
{
    public class Locations
    {
        public DateTime DateTimeUtc { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public int? Accuracy { get; set; }
    }
}
