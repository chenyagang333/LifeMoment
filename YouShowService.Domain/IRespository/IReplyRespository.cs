using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.IRespository
{
    public interface IReplyRespository
    {
        Task<(List<Reply> data, long longCount)> PagingQueryByCommentIdAsync(long commentId,int pageSize, int pageIndex);
        Task CreateAsync(Reply comment);
        Task<Reply?> QueryByIdAsync(long id);
        Task DeleteAsync(Reply comment);
        Task<List<long>> QueryLikeActiceIds(long userId, IEnumerable<long> replyIds);
    }
}
