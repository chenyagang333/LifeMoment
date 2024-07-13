using Chen.Commons;
using Chen.Infrastructure.EmailCore;
using IdentityService.Domain;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.IService;
using IdentityService.Infrastructure.Respository;
using IdentityService.Infrastructure.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<UserDomainService>();
            services.AddScoped<IUserRespository, UserRespository>();
            services.AddScoped<IUserRelevant, UserRelevant>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped(sp => new EmailBus(@"smtp.qq.com", @"2578036363@qq.com", @"hvuagjcurnytebja"));
        }
    }
}
