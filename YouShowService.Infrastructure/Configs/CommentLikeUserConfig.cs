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
    public class CommentLikeUserConfig : IEntityTypeConfiguration<CommentLikeUser>
    {
        public void Configure(EntityTypeBuilder<CommentLikeUser> builder)
        { 
            builder.ToTable("T_CommentLikeUser");
            builder.HasKey(x => new { x.UserId, x.CommentId });
            builder.HasIndex(x => new { x.UserId, x.CommentId }).IsUnique();
        }
    }
}
