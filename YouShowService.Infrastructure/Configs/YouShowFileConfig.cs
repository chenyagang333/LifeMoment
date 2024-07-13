using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Infrastructure.Configs
{
    public class YouShowFileConfig : IEntityTypeConfiguration<YouShowFile>
    {
        public void Configure(EntityTypeBuilder<YouShowFile> builder)
        {
            builder.ToTable("T_YouShowFile");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.YouShowId);
        }

    }
}
