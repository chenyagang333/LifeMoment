using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Infrastructure.Configs
{
    public class ReplyLikeUserConfig : IEntityTypeConfiguration<ReplyLikeUser>
    {
        public void Configure(EntityTypeBuilder<ReplyLikeUser> builder)
        { 
            builder.ToTable("T_ReplyLikeUser");
            builder.HasKey(x => new { x.UserId, x.ReplyId });
            builder.HasIndex(x => new { x.UserId, x.ReplyId }).IsUnique();
        }
    }
}
