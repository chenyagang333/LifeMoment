using Chen.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;
using YouShowService.Domain.IRespository;

namespace YouShowService.Infrastructure.Respository
{
    public class CommentRespository : ICommentRespository
    {
        private readonly YouShowDbContext ctx;

        public CommentRespository(YouShowDbContext ctx)
        {
            this.ctx = ctx;
        }

        public Task CreateAsync(Comment comment)
        {
            ctx.Comments.AddAsync(comment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Comment comment)
        {
            ctx.Comments.Remove(comment);
            return Task.CompletedTask;
        }

        public Task<Comment?> QueryByIdAsync(long id)
        {
            return ctx.Comments.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<(List<Comment> data,long longCount)> PagingQueryByShowIdAsync(long showId, int pageSize, int pageIndex)
        {
            var list = ctx.Comments.Where(x => x.ShowId == showId);
            var data =  await list.OrderBy(x => x.Id).Paging(pageSize, pageIndex).ToListAsync();
            var longCount = await list.LongCountAsync();
            return (data, longCount);
        }

        public Task<List<long>> QueryLikeActiceIds(long userId, IEnumerable<long> commentIds)
        {
            return ctx.CommentLikeUsers.
             Where(x => x.UserId == userId && commentIds.Contains(x.CommentId)).Select(x => x.CommentId).ToListAsync();
        }
    }
}
