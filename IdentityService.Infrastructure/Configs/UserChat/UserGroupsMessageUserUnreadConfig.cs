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
    public class UserGroupsMessageUserUnreadConfig : IEntityTypeConfiguration<UserGroupsMessageUserUnread>
    {
        public void Configure(EntityTypeBuilder<UserGroupsMessageUserUnread> builder)
        {
            builder.ToTable("User_GroupsMessageUserUnread");
            // 复合唯一索引 // 群聊Id，用来查询用户未读数量 以及 批量删除
            builder.HasIndex(x => new { x.UserGroupsId, x.ToUserId, x.UserGroupsMessageId }).IsUnique(); 
        }

    }
}
