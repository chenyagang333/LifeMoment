using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class DbContextDesignTimeFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<UserDbContext> builder = new();

            var connectionString = "server=localhost;user=root;password=AAA333;database=LifeBus_Identity_Service";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));

            builder.UseMySql(connectionString, serverVersion);
            return new UserDbContext(builder.Options);
        }

    }
}
