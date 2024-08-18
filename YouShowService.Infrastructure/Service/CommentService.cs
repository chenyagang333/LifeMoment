using AutoMapper;
using Chen.DomainCommons.ConfigOptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Entities;
using YouShowService.Domain.IRespository;
using YouShowService.Domain.IService;
using YouShowService.Infrastructure.Respository;

namespace YouShowService.Infrastructure.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRespository commentRespository;
        private readonly IMapper mapper;
        private readonly YouShowDbContext ctx;
        private readonly IReplyService replyService;
        private readonly IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions;

        public CommentService(ICommentRespository commentRespository,
            IMapper mapper,
            YouShowDbContext ctx,
            IReplyService replyService,
            IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions
            )
        {
            this.commentRespository = commentRespository;
            this.mapper = mapper;
            this.ctx = ctx;
            this.replyService = replyService;
            this.fileServerOptions = fileServerOptions;
        }
        public async Task<(List<CommentDTO> dto, long count)> PagingQueryByShowIdAsync(long userId, long showId, int pageSize, int pageIndex)
        {
            var (data, count) = await commentRespository.PagingQueryByShowIdAsync(showId, pageSize, pageIndex);
            var dto = mapper.Map<List<Comment>, List<CommentDTO>>(data);
            await AddRelevant(userId, dto);
            return (dto, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">评论关联用户Id</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async Task AddRelevant(long userId, List<CommentDTO> dto)
        {
            var fileBaseURL = fileServerOptions.Value.FileBaseUrl;
            dto.ForEach(x => x.SpliceUserAvatarURL(fileBaseURL));
            if (userId > 0)
            {
                var commentIds = dto.Select(x => x.Id);
                var likeData = await commentRespository.QueryLikeActiceIds(userId, commentIds);
                dto.ForEach(x => x.CheckLike(likeData));
            }
        }

        public async Task DeleteByIdAsync(long id)
        {
            var comment = await ctx.Comments.FirstAsync(x => x.Id == id);
            if (comment != null)
            {
                var show = await ctx.YouShows.FirstAsync(x => x.Id == comment.ShowId);
                show.AddCommentCount(-1);
                var commentLikes = await ctx.CommentLikeUsers.Where(x => x.CommentId == comment.Id).ToListAsync();
                await replyService.DeleteByCommentIdAsync(comment.Id, show);
                ctx.Remove(comment);
                ctx.RemoveRange(commentLikes);
            }
        }
        public async Task DeleteByShowIdAsync(long youshowId)
        {
            var comments = await ctx.Comments.Where(x => x.ShowId == youshowId).ToListAsync();
            if (comments != null && comments.Count > 0)
            {
                var commentIds = comments.Select(x => x.Id);
                var commentLikes = await ctx.CommentLikeUsers.Where(x => commentIds.Contains(x.CommentId)).ToListAsync();
                await replyService.DeleteByCommentsAsync(comments);
                ctx.RemoveRange(comments);
                ctx.RemoveRange(commentLikes);
            }
        }

        public async Task CreateComment(Comment comment)
        {
            await commentRespository.CreateAsync(comment);
            var show = await ctx.YouShows.FirstAsync(x => x.Id == comment.ShowId);
            show.AddCommentCount(1);
        }

        public async Task ActiveLikeComment(long userId, long commentId)
        {
            CommentLikeUser commentLikeUser = new(commentId, userId);
            await ctx.CommentLikeUsers.AddAsync(commentLikeUser);
            var comment = await ctx.Comments.FirstAsync(x => x.Id == commentId);
            comment.AddLikeCount(1);
        }

        public async Task CancelLikeComment(long userId, long commentId)
        {
            var commentLikeUser = await ctx.CommentLikeUsers.FirstAsync(x => x.UserId == userId && x.CommentId == commentId);
            ctx.CommentLikeUsers.Remove(commentLikeUser);
            var comment = await ctx.Comments.FirstAsync(x => x.Id == commentId);
            comment.AddLikeCount(-1);
        }
    }
}
