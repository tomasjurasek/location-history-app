namespace API.Models
{
    public partial class UsersController
    {
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
}
