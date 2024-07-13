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
    class RandomUserAvatarConfig : IEntityTypeConfiguration<RandomUserAvatar>
    {
        public void Configure(EntityTypeBuilder<RandomUserAvatar> builder)
        {
            builder.ToTable("T_Random_User_Avatars");
        }
    }
}
