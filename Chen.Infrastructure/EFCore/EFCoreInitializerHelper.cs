using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
{
    public static class EFCoreInitializerHelper
    {
        public static IServiceCollection AddAllDbContexts(this IServiceCollection services,
            Action<DbContextOptionsBuilder> builder,IEnumerable<Assembly> assemblies)
        {
            // AddDbContextPool不支持DbContext注入其他对象，而且使用不当有内存暴涨的问题，因此不用AddDbContextPool

            // 定义用于获取EntityFrameworkServiceCollectionExtensions.AddDbContext方法的参数类型数组。
            Type[] types = new Type[]
            {
                typeof(IServiceCollection), typeof(Action<DbContextOptionsBuilder>), 
                typeof(ServiceLifetime), typeof(ServiceLifetime)
            };

            // 获取EntityFrameworkServiceCollectionExtensions类中的AddDbContext方法
            var methodAddDbContext = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethod(nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext), 1, types); 

            foreach (var assmebly in assemblies)
            {
                Type[] typesInAsmeblly = assmebly.GetTypes();
                foreach (var dbCtxType in 
                    typesInAsmeblly.Where(t => !t.IsAbstract && typeof(DbContext).IsAssignableFrom(t)))
                {
                    var methodGenericAddDbContext = methodAddDbContext.MakeGenericMethod(dbCtxType);
                    methodGenericAddDbContext.Invoke(null, 
                        new object[] { services,builder, ServiceLifetime.Scoped, ServiceLifetime.Scoped });
                }
            }
            return services;
        }
    }
}
