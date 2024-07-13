using Chen.Commons.FunResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.IService
{
    public interface IShowService
    {
        Task<(List<YouShowDTO> dto, long count)> PagingQueryAsync(long userId, int pageSize, int pageIndex);
        Task<(List<YouShowDTO> dto, long count)> SearchPagingQueryAsync
            (long userId, string sort, string keyword, int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<(List<YouShowDTO> dto, long count)> PagingQueryByUserIdAsync(long _userId,long userId, int pageIndex, int pageSize);
        Task<(List<YouShowDTO> dto, long count)> PagingQueryByLikeOrStarAsync(long _userId, long userId, string like_or_star,int pageIndex, int pageSize);
        Task<List<YouShowDTO>> AddRelevant(long userId, List<YouShowDTO> data);
        Task<FunResult> CreateYouShowAsync(YouShow youShow);

        //Task<FunResult> CreateYouShowAsync(YouShow youShow,  // 弃用
        //    IFormFileCollection? fileCollection, CancellationToken cancellationToken = default);
        Task<FunResult> DeleteByIdAsync(long showId,long userId);
    }
}
