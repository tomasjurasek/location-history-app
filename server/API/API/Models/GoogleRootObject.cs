using System.Collections.Generic;
namespace API.Controllers
{
    public partial class UsersController
    {
        public class GoogleRootObject
        {
            public List<GoogleLocation> locations { get; set; }
        }
    }
}
