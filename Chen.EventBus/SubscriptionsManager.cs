using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.EventBus
{
    /// <summary>
    /// 订阅管理类
    /// </summary>
    class SubscriptionsManager
    {
        // key 是 eventName，值是监听这个事件的实现了 IIntegrationEventHandler 接口的类型
        private readonly Dictionary<string,List<Type>> _handlers = new Dictionary<string,List<Type>>();

        public event EventHandler<string> OnEventRemoved; 

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        /// <summary>
        /// 把 eventHandleType 类型（实现了 eventHandleType 接口）注册为监听了 eventName 事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventHandleType"></param>
        public void AddSubscription(string eventName, Type eventHandleType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }
            // 如果已经注册过，则报错
            if (_handlers[eventName].Contains(eventHandleType))
            {
                throw new ArgumentException($"Handler Type {eventHandleType} already registered for '{eventName}'",nameof(eventHandleType));
            }
            _handlers[eventName].Add(eventHandleType);
        }

        // 移除订阅
        public void RemoveSubscription(string eventName, Type handlerType)
        {
            _handlers[eventName].Remove(handlerType);
            if (!_handlers[eventName].Any()) // 没有事件订阅时通知移除绑定
            {
                _handlers.Remove(eventName);
                OnEventRemoved?.Invoke(this, eventName);
            }
        }

        /// <summary>
        /// 得到名字为 eventName 的所有监听者
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public IEnumerable<Type> GetHandlersForEvent(string eventName) => _handlers[eventName];
        /// <summary>
        /// 是否有类型监听（订阅）这个事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);
    }
}
