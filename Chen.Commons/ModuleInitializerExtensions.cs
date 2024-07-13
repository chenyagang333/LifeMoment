using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons
{
    public static class ModuleInitializerExtensions
    {
        /// <summary>
        /// 每个项目都可以自己写一些实现了IModuleInitializer接口的类，在其中注册自己需要的服务，
        /// 这样避免所有内容到入口项目中注册。  
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection RunModuleInitializers(this IServiceCollection services,
            IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                var moduleInitializerTypes = types.Where(t => !t.IsAbstract && typeof(IModuleInitializer).IsAssignableFrom(t));
                foreach (var type in moduleInitializerTypes)
                {
                    var initializer = (IModuleInitializer?)Activator.CreateInstance(type);
                    if (initializer == null)
                    {
                        throw new ApplicationException($"Cannot create {type}");
                    }
                    initializer.Initialize(services);
                }
            }
            return services;
        }
    }
}
