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
    public class YouShowConfig : IEntityTypeConfiguration<YouShow>
    {
        public void Configure(EntityTypeBuilder<YouShow> builder)
        { 
            builder.ToTable("T_YouShow");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId);
        }
    }
}
