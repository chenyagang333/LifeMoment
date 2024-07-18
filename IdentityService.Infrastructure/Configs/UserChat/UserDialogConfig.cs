using IdentityService.Domain.Entities;
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
    public class UserDialogConfig : IEntityTypeConfiguration<UserDialog>
    {
        public void Configure(EntityTypeBuilder<UserDialog> builder)
        {
            builder.ToTable("User_Dialog");
        }
    }
}
