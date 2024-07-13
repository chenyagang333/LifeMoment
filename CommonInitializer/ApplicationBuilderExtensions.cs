using Chen.EventBus;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInitializer
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseChenDefault(this IApplicationBuilder app)
        {
            app.UseEventBus();
            app.UseCors(); // 启用 Cors
            app.UseForwardedHeaders();
            //app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，而且webapi项目也没必要配置这个
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
