using IdentityService.Domain.Entities.UserChat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Configs.UserChat
{
    public class UserDialogMessageConfig : IEntityTypeConfiguration<UserDialogMessage>
    {
        public void Configure(EntityTypeBuilder<UserDialogMessage> builder)
        {
            builder.ToTable("User_DialogMessage");
            builder.HasIndex(x => new { x.UserDialogId, x.ToUserId,x.Received }); // 索引

        }
    }
}
