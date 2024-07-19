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
    public class UserGroupsToUserConfig : IEntityTypeConfiguration<UserGroupsToUser>
    {
        public void Configure(EntityTypeBuilder<UserGroupsToUser> builder)
        {
            builder.ToTable("User_GroupsToUser");
            builder.HasIndex(x => new {x.UserId,x.UserGroupsId}); // 索引 查询
            builder.HasIndex(x => x.UserGroupsId); // 索引 更新

        }
    }
}
