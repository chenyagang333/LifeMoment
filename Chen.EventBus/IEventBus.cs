using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.EventBus
{
    public interface IEventBus
    {
        void Publish(string eventName,object? eventData);
        void Subscribe(string eventName,Type handleType);
        void UnSubscribe(string eventName,Type handleType);
    }
}
