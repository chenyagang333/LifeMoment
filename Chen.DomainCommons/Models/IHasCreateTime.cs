using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.DomainCommons.Models
{
    public interface IHasCreateTime
    {
        DateTime CreateTime { get;}
    }
}
