using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.EventBus
{
    public interface IIntegrationEventHandler
    {
        // 因为消息可能会重复发送，因此 Handle 内的实现是需要幂等的
        Task Handle(string eventName, string eventData);
    }
}
