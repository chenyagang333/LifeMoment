using Chen.Commons.FunResult;
using IdentityService.Domain.Entities;
using IdentityService.Domain.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class UserRelevant : IUserRelevant
    {
        private readonly UserDbContext ctx;
        public UserRelevant(UserDbContext ctx)
        {
            this.ctx = ctx;
        }


        public async Task<bool> GetSignInStateAsync(long userId)
        {
            var data = await ctx.SignIns.
                FirstOrDefaultAsync(s => s.UserId == userId && s.SignInDate == DateTime.Today);
            if (data == null) return false;
            return true;
        }
        public async Task<FunResult> SignInAsync(long userId)
        {
            var state = await GetSignInStateAsync(userId);
            if (state) return FunResult.Failed("今天已成功签到！");
            SignIn signIn = new(userId, DateTime.Today);
            await ctx.SignIns.AddAsync(signIn);
            return FunResult.Success;
        }

        public Task<int> GetCountOfSignInForMonthAsync(long userId)
        {
            return ctx.SignIns.CountAsync(s => s.UserId == userId && s.SignInDate.Month == DateTime.Today.Month);
        }

        public async Task<string> GetAttentionStateAsync(long userId, long toUserId)
        {
            var data1 = await ctx.UserAttentionUsers.SingleOrDefaultAsync(x => x.UserId == userId && x.ToUserId == toUserId);
            var data2 = await ctx.UserAttentionUsers.SingleOrDefaultAsync(x => x.UserId == toUserId && x.ToUserId == userId);
            if (data1 == null && data2 == null)
            {
                return "00"; // 谁也没关注对方
            }
            if (data1 != null && data2 != null)
            {
                return "11"; // 互相关注
            }
            if (data1 != null)
            {
                return "10"; // 我关注TA
            }
            //if (data2 != null)
            return "01"; // TA关注我

        }

        public async Task UserAddAttentionAsync(long userId, long toUserId)
        {
            // 关注关联表添加数据
            UserAttentionUser userAttentionUser = new(userId, toUserId);
            await ctx.UserAttentionUsers.AddAsync(userAttentionUser);
            // 添加用户关注数量
            var user1 = await ctx.Users.FirstAsync(x => x.Id == userId);
            user1.AddAttentionCount(1);
            // 添加被关注用户粉丝数量
            var user2 = await ctx.Users.FirstAsync(x => x.Id == toUserId);
            user2.AddFansCount(1);
        }

        public async Task UserCancelAttentionAsync(long userId, long toUserId)
        {
            var data = await ctx.UserAttentionUsers.FirstAsync(x => x.UserId == userId && x.ToUserId == toUserId);
            ctx.UserAttentionUsers.Remove(data);
            // 减去用户关注数量
            var user1 = await ctx.Users.FirstAsync(x => x.Id == userId);
            user1.AddAttentionCount(-1);
            // 减去被关注用户粉丝数量
            var user2 = await ctx.Users.FirstAsync(x => x.Id == toUserId);
            user2.AddFansCount(-1);
        }
    }
}
