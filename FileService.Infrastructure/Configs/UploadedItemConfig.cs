using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Configs
{
    public class UploadedItemConfig : IEntityTypeConfiguration<UploadedItem>
    {
        public void Configure(EntityTypeBuilder<UploadedItem> builder)
        {
            builder.ToTable("T_FS_UploadedItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileName).IsUnicode().HasMaxLength(1024);
            builder.Property(x => x.FileSHA256Hash).IsUnicode(false).HasMaxLength(64);
            builder.HasIndex(x => new { x.FileSHA256Hash, x.FileSizeInBytes });
        }
    }
}
