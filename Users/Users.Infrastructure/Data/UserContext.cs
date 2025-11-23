using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;
using Users.Infrastructure.Data.Configurations;

namespace Users.Infrastructure.Data
{
    public class UserContext:DbContext
    {
        public class PmContext : DbContext
        {
            public PmContext(DbContextOptions options) : base(options)
            {
            }

            public DbSet<User> Users { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new UserConfiguration());
            }
        }
    }
}
