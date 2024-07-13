using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chen.EventBus
{
    public abstract class JsonIntegrationEventHandler<T> : IIntegrationEventHandler
    {
        public Task Handle(string eventName, string jsonEventData)
        {
            T? eventData = JsonSerializer.Deserialize<T>(jsonEventData);
            return HandleJson(eventName, eventData);
        }

        public abstract Task HandleJson(string eventName, T? eventData);    
    }
}
