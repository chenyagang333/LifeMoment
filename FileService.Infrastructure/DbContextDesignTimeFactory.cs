using FileService.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class DbContextDesignTimeFactory : IDesignTimeDbContextFactory<FSDbContext>
    {
        public FSDbContext CreateDbContext(string[] args)
        { 
            DbContextOptionsBuilder<FSDbContext> builder = new();

            var connectionString = "server=localhost;user=root;password=AAA333;database=LifeBus_File_Service";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));


            builder.UseMySql(connectionString, serverVersion);
            return new FSDbContext(builder.Options, null);
        }
    }
}
