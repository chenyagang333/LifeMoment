using AutoMapper;
using Chen.DomainCommons.ConfigOptions;
using Microsoft.AspNetCore.Builder;
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
    public class ReplyService : IReplyService
    {
        private readonly IReplyRespository replyRespository;
        private readonly IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions;
        private readonly IMapper mapper;
        private readonly YouShowDbContext ctx;

        public ReplyService(IMapper mapper, 
            YouShowDbContext ctx,
            IReplyRespository replyRespository,
            IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions
            )
        {
            this.mapper = mapper;
            this.ctx = ctx;
            this.replyRespository = replyRespository;
            this.fileServerOptions = fileServerOptions;
        }
        public async Task<(List<ReplyDTO> dto, long count)> PagingQueryByShowIdAsync(long userId, long commentId, int pageSize, int pageIndex)
        {
            var (data, count) = await replyRespository.PagingQueryByCommentIdAsync(commentId, pageSize, pageIndex);
            var dto = mapper.Map<List<Reply>, List<ReplyDTO>>(data);
            await AddRelevant(userId, dto);
            return (dto, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">评论关联用户Id</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async Task AddRelevant(long userId, List<ReplyDTO> dto)
        {
            var fileBaseURL = fileServerOptions.Value.FileBaseUrl;
            dto.ForEach(x => x.SpliceUserAvatarURL(fileBaseURL));
            if (userId > 0)
            {
                var replyIds = dto.Select(x => x.Id);
                var likeData = await replyRespository.QueryLikeActiceIds(userId, replyIds);
                dto.ForEach(x => x.CheckLike(likeData));
            }
        }

        public async Task DeleteByIdAsync(long id)
        {
            var reply = await ctx.Replys.FirstOrDefaultAsync(x => x.Id == id);
            if (reply != null)
            {
                var replyLikes = await ctx.ReplyLikeUsers.Where(x => x.ReplyId == reply.Id).ToListAsync();
                var comment = await ctx.Comments.FirstAsync(x => x.Id == reply.CommentId);
                comment.AddReplyCount(-1);
                var show = await ctx.YouShows.FirstAsync(x => x.Id == comment.ShowId);
                show.AddCommentCount(-1);
                ctx.Remove(reply);
                ctx.RemoveRange(replyLikes);
            }
        }


        public async Task DeleteByCommentIdAsync(long commentId, YouShow youShow)
        {
            var replies = await ctx.Replys.Where(x => x.CommentId == commentId).ToListAsync();
            youShow.AddCommentCount(-replies.Count);
            await DeleteLikesByReplIesAsync(replies);
        }

        public async Task DeleteByCommentsAsync(IEnumerable<Comment> comments)
        {
            var commentIds = comments.Select(x => x.Id);
            var replies = await ctx.Replys.Where(x => commentIds.Contains(x.CommentId)).ToListAsync();
            await DeleteLikesByReplIesAsync(replies);
        }
        public async Task DeleteLikesByReplIesAsync(IEnumerable<Reply> replies)
        {
            if (replies != null && replies.Count() > 0)
            {
                var replyIds = replies.Select(x => x.Id);
                var replyLikes = await ctx.ReplyLikeUsers.Where(x => replyIds.Contains(x.ReplyId)).ToListAsync();
                ctx.RemoveRange(replies);
                ctx.RemoveRange(replyLikes);
            }
        }

        public async Task CreateReply(Reply reply, long showId)
        {
            await replyRespository.CreateAsync(reply);
            var show = await ctx.YouShows.FirstAsync(x => x.Id == showId);
            show.AddCommentCount(1);
            var comment = await ctx.Comments.FirstAsync(x => x.Id == reply.CommentId);
            comment.AddReplyCount(1);
        }

        public async Task ActiveLikeReply(long userId, long replyId)
        {
            ReplyLikeUser replyLikeUser = new(replyId, userId);
            await ctx.ReplyLikeUsers.AddAsync(replyLikeUser);
            var reply = await ctx.Replys.FirstAsync(x => x.Id == replyId);
            reply.AddLikeCount(1);
        }

        public async Task CancelLikeReply(long userId, long replyId)
        {
            var replyLikeUser = await ctx.ReplyLikeUsers.FirstAsync(x => x.UserId == userId && x.ReplyId == replyId);
            ctx.ReplyLikeUsers.Remove(replyLikeUser);
            var reply = await ctx.Replys.FirstAsync(x => x.Id == replyId);
            reply.AddLikeCount(-1);
        }

    }

}
