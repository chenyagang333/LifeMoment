using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons
{
    public static class ComputedHelper
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> entities ,int pageSize,int pageIndex)
        {
            return entities.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }
    }
}
