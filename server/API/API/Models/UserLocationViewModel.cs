using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class UserLocationViewModel
    {
        public string Name { get; set; }
        public ICollection<LocationViewModel> Locations { get; set; }
    }
}
