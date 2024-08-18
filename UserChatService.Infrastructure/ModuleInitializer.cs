using Chen.Commons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserChatService.Domain.IService;
using UserChatService.Infrastructure.Service;

namespace UserChatService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IUserChatService, Service.UserChatService>();
            services.AddScoped<IUserDialogService, UserDialogService>();
            services.AddScoped<IUserGroupsService, UserGroupsService>();
            services.AddScoped<IUserHubService, UserHubService>();
        }
    }
}
