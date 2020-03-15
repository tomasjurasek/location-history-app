namespace API.Controllers
{
    public partial class UsersController
    {
        public class Locations
        {
            public string timestampMs { get; set; }
            public int latitudeE7 { get; set; }
            public int longitudeE7 { get; set; }
            public int accuracy { get; set; }
        }
    }
}
