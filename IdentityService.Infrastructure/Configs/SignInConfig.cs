using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Configs
{
    class SignInConfig : IEntityTypeConfiguration<SignIn>
    {
        public void Configure(EntityTypeBuilder<SignIn> builder)
        {
            builder.ToTable("T_SignIn");
            builder.HasKey(x => new { x.UserId, x.SignInDate }); // 复合主键
            builder.HasIndex(x => new { x.UserId, x.SignInDate }).IsUnique(); // 复合索引
        }

    }
}
