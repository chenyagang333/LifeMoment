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
    public class YouShowStarUserConfig : IEntityTypeConfiguration<YouShowStarUser>
    {
        public void Configure(EntityTypeBuilder<YouShowStarUser> builder)
        { 
            builder.ToTable("T_YouShowStarUser");
            builder.HasIndex(x => new { x.UserId, x.YouShowId }).IsUnique();
        }
    }
}
