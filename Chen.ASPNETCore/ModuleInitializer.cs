using Chen.Commons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.ASPNETCore
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddScoped<IMemoryCacheHelper,MemoryCacheHelper>();
            services.AddScoped<IDistributedCacheHelper,DistributedCacheHelper>();
        }
    }
}
