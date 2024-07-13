using Chen.Commons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain;
using YouShowService.Domain.IRespository;
using YouShowService.Domain.IService;
using YouShowService.Infrastructure.Respository;
using YouShowService.Infrastructure.Service;

namespace YouShowService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IYouShowRespository, YouShowRespository>();
            services.AddScoped<ICommentRespository, CommentRespository>();
            services.AddScoped<IReplyRespository, ReplyRespository>();
            services.AddScoped<IShowService, ShowService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IReplyService, ReplyService>();
        }
    }
}
