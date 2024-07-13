using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Infrastructure.EFCore
{
    public static class EFCoreHelper
    {
        /// <summary>
        /// 得到表名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string? GetTableName<TEntity>(this DbContext ctx)
        {
            var entityType = ctx.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
            {
                throw new ArgumentOutOfRangeException("TEntity is nof found in DbContext");
            }
            return entityType.GetTableName();
        }

        public static string GetColumnName<TEntity>(this DbContext ctx,Expression<Func<TEntity,object>> propertyLambda)
        {
            var entityType = ctx.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
            {
                throw new ArgumentOutOfRangeException("TEntity is nof found in DbContext");
            }

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                var unary = propertyLambda.Body as UnaryExpression;
                if (unary != null)
                {
                    member = unary.Operand as MemberExpression;
                }
            }
            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
                       propertyLambda.ToString()));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.",
                   propertyLambda.ToString()));
            }

            Type type = typeof(TEntity);
            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(), type));
            }

            string propertyName = propInfo.Name;
            var objId = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
            return entityType.FindProperty(propertyName).GetColumnName(objId.Value);
        }
    }
}
