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
            builder.HasIndex(x => new { x.ToUserId, x.UserId }); // 索引 一级索引 更新冗余数据用 // 二级索引用来查询是否存在两人的对话
        }
    }
}
