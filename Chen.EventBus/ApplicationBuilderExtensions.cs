using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.EventBus
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEventBus(this IApplicationBuilder app)
        {
            //获得一次 IEventBus 一次，就会立即加载 IEventBus，这样扫描所有的 EventHandle，保证消息及时消费。
            object? eventBus = app.ApplicationServices.GetService(typeof(IEventBus));
            if (eventBus == null)
            {
                throw new ApplicationException($"找不到IEventBus实例");
            }
            return app;
        }
    }
}
