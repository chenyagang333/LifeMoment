using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.EventBus
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class EventNameAttribute : Attribute
    {
        public EventNameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; init; }
    }
}
