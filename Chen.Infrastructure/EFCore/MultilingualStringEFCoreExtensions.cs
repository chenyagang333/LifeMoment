using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{   
    public static class MultilingualStringEFCoreExtensions
    {
        public static EntityTypeBuilder<TEntity> OwnOneMultilingualString<TEntity>
            (this EntityTypeBuilder<TEntity> entityTypeBuilder,
            Expression<Func<TEntity,MultilingualString>> navigationExpression,
            bool required = true,int maxLength = 200) where TEntity : class
        {
            entityTypeBuilder.OwnsOne(navigationExpression, dp =>
            {
                dp.Property(c => c.Chinese).IsRequired(required).HasMaxLength(maxLength).IsUnicode();
                dp.Property(c => c.English).IsRequired(required).HasMaxLength(maxLength).IsUnicode();
            });
            entityTypeBuilder.Navigation(navigationExpression).IsRequired(required);
            return entityTypeBuilder;
        }
    }
}
