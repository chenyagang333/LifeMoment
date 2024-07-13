using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.IService
{
    public interface ICommentService
    {
        Task<(List<CommentDTO> dto, long count)> PagingQueryByShowIdAsync(long userId, long showId, int pageSize, int pageIndex);

        Task DeleteByIdAsync(long id);
        Task DeleteByShowIdAsync(long youshowId);
        Task CreateComment(Comment comment);
        Task ActiveLikeComment(long userId,long commentId);
        Task CancelLikeComment(long userId,long commentId);


    }
}
