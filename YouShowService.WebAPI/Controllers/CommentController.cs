using AutoMapper;
using Chen.ASPNETCore;
using Chen.Commons;
using Chen.Commons.ApiResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouShowService.Domain.Entities;
using YouShowService.Domain.IRespository;
using YouShowService.Domain.IService;
using YouShowService.Domain.RequestObject;
using YouShowService.Infrastructure;
using YouShowService.Infrastructure.Respository;

namespace YouShowService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(YouShowDbContext))]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRespository commentRespository;
        private readonly ICommentService commentService;
        private readonly IMapper mapper;
        private readonly YouShowDbContext ctx;

        public CommentController(IMapper mapper, ICommentRespository commentRespository, ICommentService commentService, YouShowDbContext ctx)
        {
            this.mapper = mapper;
            this.commentRespository = commentRespository;
            this.commentService = commentService;
            this.ctx = ctx;
        }
        [HttpGet]
        public async Task<ActionResult<ApiListResult>> PagingQueryByShowId(long showId, int pageSize, int pageIndex)
        {
            var userId = HttpHelper.TryGetUserId(HttpContext);
            var (data, count) = await commentService.PagingQueryByShowIdAsync(userId, showId, pageSize, pageIndex);
            return ApiListResult.Succeeded(data, count);
        }
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateComment(CreateComment createComment)
        {
            Comment comment = mapper.Map<CreateComment,Comment>(createComment);
            var userId = HttpHelper.GetUserId(HttpContext);
            comment.UpdateUserId(userId);
            await commentService.CreateComment(comment);
            await ctx.SaveChangesAsync();
            return ApiResult.Succeeded(comment.Id);
        }
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteByIdAsync(long id)
        {
            await commentService.DeleteByIdAsync(id);
            return ApiResult.Succeess;
        }
        [HttpGet]
        // 添加点赞
        public async Task<ActionResult<ApiResult>> UpdateLikeComment(long id, int updateType)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            {
                if (updateType == 1)
                {
                    await commentService.ActiveLikeComment(userId, id);
                }
                else if (updateType == 0)
                {
                    await commentService.CancelLikeComment(userId, id);
                }
                return ApiResult.Succeess;
            }
        }
    }
}
