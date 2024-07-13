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
using YouShowService.Infrastructure.Service;

namespace YouShowService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(YouShowDbContext))]
    public class ReplyController : ControllerBase
    {
        private readonly IReplyRespository replyRespository;
        private readonly IReplyService replyService;
        private readonly IMapper mapper;
        private readonly YouShowDbContext ctx;

        public ReplyController(IMapper mapper, IReplyRespository replyRespository, IReplyService replyService, YouShowDbContext ctx)
        {
            this.mapper = mapper;
            this.replyRespository = replyRespository;
            this.replyService = replyService;
            this.ctx = ctx;
        }
        [HttpGet]
        public async Task<ActionResult<ApiListResult>> PagingQueryByCommentId(int pageSize, int pageIndex, long commentId)
        {
            var userId = HttpHelper.TryGetUserId(HttpContext);
            var (data, count) = await replyService.PagingQueryByShowIdAsync(userId, commentId, pageSize, pageIndex);
            return ApiListResult.Succeeded(data, count);
        }
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateReply(CreateReply createReply)
        {
            Reply reply = mapper.Map<CreateReply,Reply>(createReply);
            var userId = HttpHelper.GetUserId(HttpContext);
            reply.UpdateUserId(userId);
            await replyService.CreateReply(reply, createReply.ShowId);
            await ctx.SaveChangesAsync();
            return ApiResult.Succeeded(reply.Id);
        }
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteByIdAsync(long id)
        {
            await replyService.DeleteByIdAsync(id);
            return ApiResult.Succeess;
        }

        [HttpGet]
        // 添加点赞
        public async Task<ActionResult<ApiResult>> UpdateLikeReply(long id, int updateType)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            if (updateType == 1)
            {
                await replyService.ActiveLikeReply(userId, id);
            }
            else
            {
                await replyService.CancelLikeReply(userId, id);
            }
            return ApiResult.Succeess;
        }
    }
}
