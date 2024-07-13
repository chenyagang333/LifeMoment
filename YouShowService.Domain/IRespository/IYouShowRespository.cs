using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.IRespository
{
    public interface IYouShowRespository
    {
        Task<(List<YouShow>, long count)> PagingQueryAsync(int pageSize, int pageIndex);
        Task CreateAsync(YouShow youShow);
        Task DeleteAsync(YouShow youShow);
        Task<YouShow?> QueryByIdAsync(long Id);
        Task<List<long>> QueryLikeActiceIds(long userId,IEnumerable<long> showIds);
        Task<List<long>> QueryStarActiceIds(long userId,IEnumerable<long> showIds);

    }
}
