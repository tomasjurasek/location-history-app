using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Database.Entities
{
    public class UserLocations : TableEntity
    {
        public UserLocations()
        {
        }
        public string JsonLocations { get; set; }
    }
}
