using AutoMapper;
using Chen.ASPNETCore;
using Chen.Commons;
using Chen.Commons.ApiResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using YouShowService.Domain;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Entities;
using YouShowService.Domain.IRespository;
using YouShowService.Domain.IService;
using YouShowService.Domain.RequestObject;
using YouShowService.Infrastructure;
using YouShowService.Infrastructure.Respository;

namespace YouShowService.WebAPI.Controllers
{
    [Authorize]
    [UnitOfWork(typeof(YouShowDbContext))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class YouShowController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IYouShowRespository youShowRespository;
        private readonly IMapper mapper;
        private readonly IShowService youShowService;
        private readonly YouShowDbContext ctx;
        private readonly IWebHostEnvironment _env;

        public YouShowController(IYouShowRespository youShowRespository, IMapper mapper, IHttpContextAccessor httpContextAccessor, YouShowDbContext ctx, IShowService youShowService, IWebHostEnvironment _env)
        {
            this.youShowRespository = youShowRespository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.ctx = ctx;
            this.youShowService = youShowService;
            this._env = _env;
        }

        [AllowAnonymous]
        [HttpGet]
        // 分页查询 youshow
        public async Task<ActionResult<ApiResult>> PagingQueryAsync(int pageSize, int pageIndex)
        {
            var userId = HttpHelper.TryGetUserId(HttpContext);
            var (data, count) = await youShowService.PagingQueryAsync(userId, pageSize, pageIndex);
            return ApiListResult.Succeeded(data, count);
        }
        [AllowAnonymous]
        [HttpGet]
        // 根据关键字关键字 分页查询 youshow
        public async Task<ActionResult<ApiResult>> SearchPagingQueryAsync([FromQuery] SearchYouShowRequest req, CancellationToken cancellationToken)
        {
            var userId = HttpHelper.TryGetUserId(HttpContext);
            var (data, count) = await youShowService
                .SearchPagingQueryAsync(userId, req.sort, req.keyword, req.pageIndex, req.pageSize, cancellationToken);
            return ApiListResult.Succeeded(data, count);
        }

        [AllowAnonymous]
        [HttpGet]
        // 根据用户Id 分页查询 youshow
        public async Task<ActionResult<ApiResult>> PagingQueryByUserId(long userId, int pageIndex, int pageSize)
        {
            if (userId > 0)
            {
                var _userId = HttpHelper.TryGetUserId(HttpContext);
                var (data, count) = await youShowService
                    .PagingQueryByUserIdAsync(_userId,userId, pageIndex, pageSize);
                return ApiListResult.Succeeded(data, count);
            }
            return ApiResult.Error;
        }
        [AllowAnonymous]
        [HttpGet]
        // 根据用户Id 分页查询 youshow
        public async Task<ActionResult<ApiResult>> PagingQueryByLikeOrStar(long userId, string like_or_star, int pageIndex, int pageSize)
        {
            if (userId > 0)
            {
                var _userId = HttpHelper.TryGetUserId(HttpContext);
                var (data, count) = await youShowService
                    .PagingQueryByLikeOrStarAsync(_userId,userId, like_or_star, pageIndex, pageSize);
                return ApiListResult.Succeeded(data, count);
            }
            return ApiResult.Error;
        }



        [HttpPost]
        // 创建youshow
        public async Task<ActionResult<ApiResult>> CreateYouShowAsync(CreateYouShow createYouShow)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            YouShow youShow = mapper.Map<CreateYouShow, YouShow>(createYouShow);

            youShow.UpdateUserId(userId);
            var res = await youShowService.CreateYouShowAsync(youShow);
            if (res.Succeeded)
            {
                int mark = await ctx.SaveChangesAsync();
                if (mark > 0)
                {
                    // 如果存在则添加文件路径
                    if (createYouShow.Files != null)
                    {
                        createYouShow.Files.ForEach(x => x.UpdateYouShowId(youShow.Id));
                        await ctx.YouShowFiles.AddRangeAsync(createYouShow.Files);
                    }
                    // 发布领域事件，通知SearchService存储新增的数据
                    youShow.Build().AddUserYouShowCount(1);
                    return ApiResult.Succeeded(youShow.Id);
                }
                return ApiResult.Failed("作品发布失败，请重试！");
            }
            return ApiResult.Failed(res.Description);
        }

        [HttpDelete]
        // 根据Id删除youshow
        public async Task<ActionResult<ApiResult>> DeleteByIdAsync(long id)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            await youShowService.DeleteByIdAsync(userId, id);
            return ApiResult.Succeess;
        }
        [HttpGet]
        // 获取IP所在的省份
        public async Task<ActionResult<ApiResult>> GetAddress()
        {
            try
            {
                var res = await HttpHelper.GetAddressByHttpContextAccessor(httpContextAccessor);
                return ApiResult.Succeeded(res);
            }
            catch (Exception)
            {
                return ApiResult.Error;
            }
            //var addressIp = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            //string? address = await HttpHelper.GetAddressByRemoteIpAddress("114.247.50.2");
            //return ApiResult.Succeeded(address);
        }
        [HttpGet]
        // 添加收藏
        public async Task<ActionResult<ApiResult>> StarShow(long youshowId)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            var entity = new YouShowStarUser(youshowId, userId).AddUserStarCount(1);
            await ctx.YouShowStarUsers.AddAsync(entity);
            var youshow = await youShowRespository.QueryByIdAsync(youshowId);
            youshow?.AddStarCount(1);
            return ApiResult.Succeess;
        }
        [HttpGet]
        // 取消收藏
        public async Task<ActionResult<ApiResult>> CancelStarShow(long youshowId)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            var entity = await ctx.YouShowStarUsers
                .FirstOrDefaultAsync(x => x.YouShowId == youshowId && x.UserId == userId);
            if (entity != null)
            {
                ctx.YouShowStarUsers.Remove(entity.AddUserStarCount(-1));
                var youshow = await youShowRespository.QueryByIdAsync(youshowId);
                youshow?.AddStarCount(-1);
                return ApiResult.Succeess;
            }
            return ApiResult.Failed("#已经取消点赞了");
        }
        [HttpGet]
        // 添加点赞
        public async Task<ActionResult<ApiResult>> LikeShow(long youshowId)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            var entity = new YouShowLikeUser(youshowId, userId).AddUserLikeCount(1);
            await ctx.YouShowLikeUsers.AddAsync(entity);
            var youshow = await youShowRespository.QueryByIdAsync(youshowId);
            youshow?.AddLikeCount(1);
            return ApiResult.Succeess;
        }
        [HttpGet]
        // 取消点赞
        public async Task<ActionResult<ApiResult>> CancelLikeShow(long youshowId)
        {
            var userId = HttpHelper.GetUserId(HttpContext);
            var entity = await ctx.YouShowLikeUsers
            .FirstOrDefaultAsync(x => x.YouShowId == youshowId && x.UserId == userId);
            if (entity != null)
            {
                ctx.YouShowLikeUsers.Remove(entity.AddUserLikeCount(-1));
                var youshow = await youShowRespository.QueryByIdAsync(youshowId);
                youshow?.AddLikeCount(-1);
                return ApiResult.Succeess;
            }
            return ApiResult.Failed("#已经取消点赞了");
        }


        #region 先上传图片再上传说说，串行操作，浪费时间，弃用该方法

        /// <summary>
        /// 先上传图片再上传说说，串行操作，浪费时间，弃用该方法
        /// </summary>
        /// <param name="createYouShow"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //[HttpPost]
        //// 创建youshow
        //public async Task<ActionResult<ApiResult>> CreateYouShowAsync([FromForm] CreateYouShow createYouShow, CancellationToken cancellationToken)
        //{
        //    var userId = HttpHelper.GetUserId(HttpContext);
        //    YouShow youShow = mapper.Map<YouShow>(createYouShow);
        //    youShow.UpdateUserId(userId);
        //    var res = await youShowService.CreateYouShowAsync(youShow, createYouShow.FileCollection, cancellationToken);
        //    if (res.Succeeded)
        //    {
        //        int mark = await ctx.SaveChangesAsync();
        //        if (mark > 0)
        //        {
        //            return ApiResult.Succeeded(youShow.Id);
        //        }
        //        return ApiResult.Failed("作品发布失败，请重试！");
        //    }
        //    return ApiResult.Failed(res.Description);
        //}

        #endregion

    }
}