using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Configs
{
    class UserAttentionUserConfig : IEntityTypeConfiguration<UserAttentionUser>
    {
        public void Configure(EntityTypeBuilder<UserAttentionUser> builder)
        {
            builder.ToTable("T_User_Attention_User");
            builder.HasKey(x => new { x.UserId, x.ToUserId }); // 复合主键
            builder.HasIndex(x => new { x.UserId, x.ToUserId }).IsUnique(); // 复合唯一索引
        }
    }
}
