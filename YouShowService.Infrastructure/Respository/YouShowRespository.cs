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
    public class YouShowRespository : IYouShowRespository
    {
        private readonly YouShowDbContext ctx;

        public YouShowRespository(YouShowDbContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task CreateAsync(YouShow youShow)
        {
            await ctx.YouShows.AddAsync(youShow);
        }

        public Task DeleteAsync(YouShow youShow)
        {
            ctx.YouShows.Remove(youShow);
            return Task.CompletedTask;
        }

        public Task<YouShow?> QueryByIdAsync(long Id)
        {
            return ctx.YouShows.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<(List<YouShow>, long count)> PagingQueryAsync(int pageSize, int pageIndex)
        {
            var data = await ctx.YouShows.OrderByDescending(x => x.Id).Paging(pageSize, pageIndex).ToListAsync();
            var count = await ctx.YouShows.LongCountAsync();
            return (data, count); 
        }

        public Task<List<long>> QueryLikeActiceIds(long userId, IEnumerable<long> showIds)
        {
            return ctx.YouShowLikeUsers.
             Where(x => x.UserId == userId && showIds.Contains(x.YouShowId)).Select(x => x.YouShowId).ToListAsync();
        }

        public Task<List<long>> QueryStarActiceIds(long userId, IEnumerable<long> showIds)
        {
            return ctx.YouShowStarUsers.
            Where(x => x.UserId == userId && showIds.Contains(x.YouShowId)).Select(x => x.YouShowId).ToListAsync();
        }
    }
}
