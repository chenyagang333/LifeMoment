using SearchService.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Domain.IRepository
{
    public interface ISearchUserRepository
    {
        Task InsertAsync(LifeBusUser user);
        Task UpdateAsync(long id, object obj);
        Task DeleteAsync(long id);
        Task<(IEnumerable<LifeBusUser>, long totalCount)>
            SearchUsers(string sort, string keyword, int pageIndex, int pageSize);
    }
}
