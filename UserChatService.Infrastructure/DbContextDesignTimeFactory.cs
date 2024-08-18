using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Infrastructure
{
    public class DbContextDesignTimeFactory : IDesignTimeDbContextFactory<UserChatDbContext>
    {
        public UserChatDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<UserChatDbContext> builder = new();

            var connectionString = "server=localhost;user=root;password=AAA333;database=LifeBus_UserChat_Service";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));

            builder.UseMySql(connectionString, serverVersion);
            return new UserChatDbContext(builder.Options,null);
        }
    }
}
