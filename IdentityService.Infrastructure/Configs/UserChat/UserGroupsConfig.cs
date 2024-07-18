using IdentityService.Domain.Entities.UserChat;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Configs.UserChat
{
    public class UserGroupsConfig : IEntityTypeConfiguration<UserGroups>
    {
        public void Configure(EntityTypeBuilder<UserGroups> builder)
        {
            builder.ToTable("User_Groups");
        }
    }
}
