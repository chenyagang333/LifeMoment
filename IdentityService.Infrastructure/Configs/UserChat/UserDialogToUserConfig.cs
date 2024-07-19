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
    public class UserDialogToUserConfig : IEntityTypeConfiguration<UserDialogToUser>
    {
        public void Configure(EntityTypeBuilder<UserDialogToUser> builder)
        {
            builder.ToTable("User_DialogToUser");
            builder.HasIndex(x => new { x.UserId, x.UserDialogId }); // 索引 根据用户Id查询
            builder.HasIndex(x => x.ToUserId); // 索引 更新用
        }
    }
}
