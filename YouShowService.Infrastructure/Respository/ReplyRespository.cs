using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;
using Chen.Commons;
using YouShowService.Domain.IRespository;
using Microsoft.EntityFrameworkCore;

namespace YouShowService.Infrastructure.Respository
{
    public class ReplyRespository : IReplyRespository
    {
        private readonly YouShowDbContext ctx;

        public ReplyRespository(YouShowDbContext ctx)
        {
            this.ctx = ctx;
        }

        public Task CreateAsync(Reply reply)
        {
            ctx.Replys.AddAsync(reply);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Reply reply)
        {
            ctx.Replys.Remove(reply);
            return Task.CompletedTask;
        }

        public Task<Reply?> QueryByIdAsync(long id)
        {
            return ctx.Replys.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<(List<Reply> data,long longCount)> PagingQueryByCommentIdAsync(long commentId, int pageSize, int pageIndex)
        {
            var list = ctx.Replys.Where(x => x.CommentId == commentId);
            var data = await list.OrderBy(x => x.Id).Paging(pageSize, pageIndex).ToListAsync();
            var longCount = await list.LongCountAsync();
            return (data, longCount);
        }

        public Task<List<long>> QueryLikeActiceIds(long userId, IEnumerable<long> replyIds)
        {
            return ctx.ReplyLikeUsers.
             Where(x => x.UserId == userId && replyIds.Contains(x.ReplyId)).Select(x => x.ReplyId).ToListAsync();
        }
    }
}
