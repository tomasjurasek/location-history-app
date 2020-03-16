using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Database.Entities
{
    public class UserLocations 
    {
        public Guid Id { get; set; }
        public string UserIdentifier { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
    }
}
