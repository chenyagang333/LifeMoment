using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.IService
{
    public interface IReplyService
    {
        Task<(List<ReplyDTO> dto, long count)> PagingQueryByShowIdAsync(long userId, long commentId, int pageSize, int pageIndex);
        Task DeleteByIdAsync(long id);
        Task DeleteByCommentIdAsync(long id,YouShow youShow);
        Task DeleteByCommentsAsync(IEnumerable<Comment> comments);
        Task DeleteLikesByReplIesAsync(IEnumerable<Reply> replies);
        Task CreateReply(Reply reply,long showId);
        Task ActiveLikeReply(long userId, long replyId);
        Task CancelLikeReply(long userId, long replyId);
    }
}
