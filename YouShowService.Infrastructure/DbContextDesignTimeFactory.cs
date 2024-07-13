using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Infrastructure;

namespace IdentityService.Infrastructure
{
    public class DbContextDesignTimeFactory : IDesignTimeDbContextFactory<YouShowDbContext>
    {
        public YouShowDbContext CreateDbContext(string[] args)
        { 
            DbContextOptionsBuilder<YouShowDbContext> builder = new();

            var connectionString = "server=localhost;user=root;password=AAA333;database=LifeBus_YouShow_Service";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));


            builder.UseMySql(connectionString, serverVersion);
            return new YouShowDbContext(builder.Options, null);
        }
    }
}
