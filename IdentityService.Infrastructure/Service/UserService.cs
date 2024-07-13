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
    public class UserService : IUserService
    {
        private readonly UserDbContext userDbContext;

        public UserService(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        public async Task<string> GetRandomAvatarAsync()
        {
            var userAvatarCount = await userDbContext.RandomUserAvatars.CountAsync();
            return await userDbContext.RandomUserAvatars
              .Skip(GetRandomIndex(userAvatarCount))
              .Take(1).Select(s => s.AvatarUrl).FirstAsync();
        }

        public async Task<string> GetRandomUserNameAsync()
        {
            var userNameCount = await userDbContext.RandomUserNames.CountAsync();
            return await userDbContext.RandomUserNames
                .Skip(GetRandomIndex(userNameCount))
                .Take(1).Select(s => s.UserName).FirstAsync();
        }

        private int GetRandomIndex(int maxCount)
        {
            return Random.Shared.Next(0, maxCount);
        }
    }
}
