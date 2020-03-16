using API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace API.Database
{
    public class LocationHistoryDbContext : DbContext
    {
        public LocationHistoryDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserLocations> UsersLocations { get; set; }
    }
}
