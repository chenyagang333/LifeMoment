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
    class RandomUserNameConfig : IEntityTypeConfiguration<RandomUserName>
    {
        public void Configure(EntityTypeBuilder<RandomUserName> builder)
        {
            builder.ToTable("T_Random_User_Names");

        }
    }
}
