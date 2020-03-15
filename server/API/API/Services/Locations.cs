using System;

namespace API.Services
{
    public class Locations
    {
        public DateTime DateTimeUtc { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public int? Accuracy { get; set; }
        public string Activity { get; set; }
        public string LocationSource { get; set; }
    }

    public enum LocationSource
    {
        Google = 1
    }
}
