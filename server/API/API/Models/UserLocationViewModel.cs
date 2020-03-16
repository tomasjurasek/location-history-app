using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class UserLocationViewModel
    {
        public string Id { get; set; }
        public ICollection<LocationViewModel> Locations { get; set; }
    }
}
