using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.IRespository
{
    public interface ICommentRespository
    {
        Task<(List<Comment> data,long longCount)> PagingQueryByShowIdAsync( long showId, int pageSize, int pageIndex);
        Task<Comment?> QueryByIdAsync(long id);
        Task CreateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
        Task<List<long>> QueryLikeActiceIds(long userId, IEnumerable<long> commentIds);
    }
}
