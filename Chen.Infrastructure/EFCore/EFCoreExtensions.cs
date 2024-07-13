using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
{
    public static class EFCoreExtensions
    {

        public static void EnableSoftDeletionGlobalFilter(this ModelBuilder modelBuilder)
        {
            var entityTypesHasSoftDeletion = modelBuilder.Model.GetEntityTypes()
                .Where(e => e.ClrType.IsAssignableTo(typeof(ISoftDelete)));

            foreach (var entityType in entityTypesHasSoftDeletion)
            {
                var isDeleteProperty = entityType.FindProperty(nameof(ISoftDelete.IsDeleted));
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var filter = Expression.Lambda(
                    Expression.Not(
                        Expression.Property(parameter, isDeleteProperty.PropertyInfo)
                        ), parameter);
                entityType.SetQueryFilter(filter);
            }
        }

        public static IQueryable<T> Query<T>(this DbContext ctx) where T : class, IEntity
        {
            return ctx.Set<T>().AsNoTracking();
        }

    }
}
