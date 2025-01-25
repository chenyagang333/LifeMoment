using AutoMapper;
using Chen.Commons;
using Chen.Commons.ApiResult;
using Chen.Commons.ApiResult.Generic;
using Chen.Commons.FunResult;
using Chen.DomainCommons.ConfigOptions;
using Chen.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Entities;
using YouShowService.Domain.Enums;
using YouShowService.Domain.IRespository;
using YouShowService.Domain.IService;
using YouShowService.Domain.Options;

namespace YouShowService.Infrastructure.Service
{
    public class ShowService : IShowService
    {
        private readonly IYouShowRespository youShowRespository;
        private readonly IMapper mapper;
        private readonly YouShowDbContext ctx;
        private readonly ICommentService commentService;
        private readonly IReplyService replyService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ITokenService tokenService;
        private readonly IOptionsSnapshot<JWTOptions> jWTOptions;
        private readonly IOptionsSnapshot<FileServiceOptions> optionsSnapshot;
        private readonly IOptionsSnapshot<SearchServiceOptions> searchServiceOptions;
        private readonly ILogger<ShowService> logger;
        private readonly IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions;

        public ShowService(IYouShowRespository youShowRespository, IMapper mapper,
            YouShowDbContext ctx, ICommentService commentService, IReplyService replyService,
            IHttpClientFactory httpClientFactory, ITokenService tokenService,
            IOptionsSnapshot<JWTOptions> jWTOptions,
            IOptionsSnapshot<FileServiceOptions> optionsSnapshot,
            IOptionsSnapshot<SearchServiceOptions> searchServiceOptions,
            ILogger<ShowService> logger,
            IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions
            )
        {
            this.youShowRespository = youShowRespository;
            this.mapper = mapper;
            this.ctx = ctx;
            this.commentService = commentService;
            this.replyService = replyService;
            this.httpClientFactory = httpClientFactory;
            this.tokenService = tokenService;
            this.jWTOptions = jWTOptions;
            this.optionsSnapshot = optionsSnapshot;
            this.searchServiceOptions = searchServiceOptions;
            this.logger = logger;
            this.fileServerOptions = fileServerOptions;
        }

        #region 作品查询

        public async Task<(List<YouShowDTO> dto, long count)> PagingQueryAsync(long userId, int pageSize, int pageIndex)
        {
            var (data, count) = await youShowRespository.PagingQueryAsync(pageSize, pageIndex);
            var dto = mapper.Map<List<YouShow>, List<YouShowDTO>>(data);
            await AddRelevant(userId, dto);
            return (dto, count);
        }
        // 根据关键字查询
        public async Task<(List<YouShowDTO> dto, long count)> SearchPagingQueryAsync
            (long userId, string sort, string keyword, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient();
            var baseUrl = searchServiceOptions.Value.UrlRoot;
            var relativeUrl = $"/api/Search/SearchStrengths?sort={sort}&keyword={keyword}&pageIndex={pageIndex}&pageSize={pageSize}";
            var requestUrl = new Uri(baseUrl, relativeUrl);
            var res = await client.GetJsonAsync<ApiListResult<List<YouShow>>>(requestUrl, cancellationToken);
            if (res.Code == 200)
            {
                var dto = mapper.Map<List<YouShow>, List<YouShowDTO>>(res.Data);
                if (userId > 0)
                {
                    await AddRelevant(userId, dto);
                    return (dto, res.Total);
                }
                return (dto, res.Total);
            }
            return default;

        }

        // 根据UserId查询
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_userId">登录用户Id</param>
        /// <param name="userId">作品关联用户Id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<(List<YouShowDTO> dto, long count)> PagingQueryByUserIdAsync(long _userId, long userId, int pageIndex, int pageSize)
        {
            var data = await ctx.YouShows.Where(x => x.UserId == userId).OrderByDescending(od => od.Id).Paging(pageSize, pageIndex).ToListAsync();
            var count = await ctx.YouShows.LongCountAsync();
            var dto = mapper.Map<List<YouShow>, List<YouShowDTO>>(data);
            await AddRelevant(_userId, dto);
            return (dto, count);
        }

        public async Task<(List<YouShowDTO> dto, long count)> PagingQueryByLikeOrStarAsync(long _userId, long userId, string like_or_star, int pageIndex, int pageSize)
        {
            long[] youshowIds = [];
            long count;
            if (like_or_star == "like")
            {
                var baseQuery = ctx.YouShowLikeUsers.Where(x => x.UserId == userId);
                youshowIds = await baseQuery.
                    OrderByDescending(od => od.Id).
                    Select(s => s.YouShowId).
                    Paging(pageSize, pageIndex).ToArrayAsync();
                count = await baseQuery.LongCountAsync();
            }
            else //  if (like_or_star == "star")
            {
                var baseQuery = ctx.YouShowStarUsers.Where(x => x.UserId == userId);
                youshowIds = await baseQuery.
                  OrderByDescending(od => od.Id).
                  Select(s => s.YouShowId).
                  Paging(pageSize, pageIndex).ToArrayAsync();
                count = await baseQuery.LongCountAsync();
            }
            // 如果 ids 存在，我们的ids是有固定顺序的
            if (youshowIds.Length > 0)
            {
                var data = await ctx.YouShows.Where(x => youshowIds.Contains(x.Id)).ToListAsync();
                var dto = mapper.Map<List<YouShow>, List<YouShowDTO>>(data);
                List<YouShowDTO> orderDTO = new List<YouShowDTO>();
                foreach (var youshowId in youshowIds) // 根据ids固定的顺序重新调整数据的数据
                {
                    var item = dto.FirstOrDefault(x => x.Id == youshowId);
                    if (item != null) orderDTO.Add(item);
                }
                await AddRelevant(_userId, orderDTO);
                return (orderDTO, count);
            }
            return ([], 0);
        }

        /// <summary>
        /// 更新文章表数据库原始数据为前端需要展示的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<YouShowDTO>> AddRelevant(long userId, List<YouShowDTO> dto)
        {
            // 拿到文章关联的所有图片或视频
            var files = await ctx.YouShowFiles.Where(c => dto.Select(x => x.Id).Contains(c.YouShowId)).ToListAsync();
            // 拿到文件存放的BaseURL
            var fileBaseURL = fileServerOptions.Value.FileBaseUrl;
            // 循环赋值
            dto.ForEach(d => d.UpdateFiles(files, fileBaseURL).SpliceUserAvatarURL(fileBaseURL));
            if (userId > 0)
            {
                var showIds = dto.Select(x => x.Id);
                var likeData = await youShowRespository.QueryLikeActiceIds(userId, showIds);
                var starData = await youShowRespository.QueryStarActiceIds(userId, showIds);
                dto.ForEach(d => d.CheckLikeStar(likeData, starData));
            }
            return dto;
        }

        #endregion

        public async Task<FunResult> CreateYouShowAsync(YouShow youShow)
        {
            await youShowRespository.CreateAsync(youShow);
            return FunResult.Success;
        }

        public async Task<FunResult> DeleteByIdAsync(long userId, long showId)
        {
            var youshow = await youShowRespository.QueryByIdAsync(showId);
            if (youshow == null) return FunResult.Failed("该作品已经删除！");
            if (youshow.UserId != userId) return FunResult.Failed("这不是您的作品，您无法删除！");
            youshow.SoftDelete(); // 删除作品 // 软删除
            youshow.Delete(); // 
            return FunResult.Success;
        }

        // 真正的删除数据
        private async Task RealDeleteAsync(YouShow youshow)
        {
            await youShowRespository.DeleteAsync(youshow); // 删除作品 // 真实删除数据
            // 删除作品对应的点赞收藏
            var likes = await ctx.YouShowLikeUsers.Where(x => x.YouShowId == youshow.Id).ToListAsync();
            var stars = await ctx.YouShowStarUsers.Where(x => x.YouShowId == youshow.Id).ToListAsync();
            ctx.RemoveRange(likes);
            ctx.RemoveRange(stars);
            // 删除作品的评论
            await commentService.DeleteByShowIdAsync(youshow.Id);
            // 删除该作品关联的文件路径信息
            var files = await ctx.YouShowFiles.Where(x => x.YouShowId == youshow.Id).ToListAsync();
            if (files != null && files.Count > 0)
            {
                ctx.YouShowFiles.RemoveRange(files);
            }

        }

        public async Task<FunResult> PutActiveAsync(long userId, long youshowId, YouShowActive active, bool isActive)
        {
            var count = isActive ? 1 : -1;
            if (isActive)
            {
                var entity = new YouShowStarUser(youshowId, userId).AddUserStarCount(1);
                await ctx.YouShowStarUsers.AddAsync(entity);
            }
            else
            {
                if (active == YouShowActive.Like)
                {
                    var entity = await ctx.YouShowLikeUsers
                        .FirstOrDefaultAsync(x => x.YouShowId == youshowId && x.UserId == userId);
                    if (entity == null) return FunResult.Failed("已经取消点赞了");
                    entity.AddUserLikeCount(count); // 通知Identity更新点赞数
                    await ctx.SaveChangesAsync(); // 在这里要保存更改，避免Remove后无法触发AddUserStarCount内的领域事件
                    ctx.YouShowLikeUsers.Remove(entity);
                }
                else if (active == YouShowActive.Star)
                {
                    var entity = await ctx.YouShowStarUsers
                        .FirstOrDefaultAsync(x => x.YouShowId == youshowId && x.UserId == userId);
                    if (entity == null) return FunResult.Failed("已经取消收藏了");
                    entity.AddUserStarCount(count);
                    await ctx.SaveChangesAsync(); // 在这里要保存更改，避免Remove后无法触发AddUserStarCount内的领域事件
                    ctx.YouShowStarUsers.Remove(entity);
                }
            }
            StringBuilder description = new(isActive ? "" : "取消");
            var youshow = await youShowRespository.QueryByIdAsync(youshowId);
            if (active == YouShowActive.Like)
            {
                youshow!.AddLikeCount(count);
                description.Append("点赞");
            }
            else if (active == YouShowActive.Star)
            {
                youshow!.AddStarCount(count);
                description.Append("收藏");
            }
            description.Append("成功！");
            return FunResult.Succeed(description.ToString());
        }




        #region 先上传图片再上传说说，串行操作，浪费时间，弃用该方法
        /// <summary>
        /// 先上传图片再上传说说
        /// </summary>
        /// <param name="youShow"></param>
        /// <param name="fileCollection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //public async Task<FunResult> CreateYouShowAndUploadFileAsync(YouShow youShow,
        // IFormFileCollection? fileCollection, CancellationToken cancellationToken = default)
        //{
        //    // 是否有图片或视频
        //    if (fileCollection != null && fileCollection.Count > 0)
        //    {
        //        Uri serveRoot = new(optionsSnapshot.Value.UrlRoot);
        //        FileServiceClient client = new(httpClientFactory, serveRoot, jWTOptions.Value, tokenService);
        //        try
        //        {
        //            var res = await client.UploadFormFilesAsync(fileCollection, 2, cancellationToken);
        //            //var res = response.ParseJson<ApiResult>()!;
        //            if (res.Code == 200)
        //            {
        //                youShow.UpdateFileURLList((IEnumerable<string>)res.Data!);
        //            }
        //            else
        //            {
        //                return FunResult.Fail;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.LogError($"文件上传失败，异常信息：{ex.Message}");
        //            return FunResult.Failed("文件上传失败");
        //        }
        //    }
        //    await youShowRespository.CreateAsync(youShow);
        //    return FunResult.Success;
        //}
        //
        #endregion

    }
}
