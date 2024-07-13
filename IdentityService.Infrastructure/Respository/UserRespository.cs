using IdentityService.Domain.Entities;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Respository
{
    public class UserRespository : IUserRespository
    {
        private readonly UserDbContext userDbContext;
        private readonly IdUserManager idUserManager;
        private readonly RoleManager<Role> roleManager;

        public UserRespository(UserDbContext userDbContext, IdUserManager idUserManager)
        {
            this.userDbContext = userDbContext;
            this.idUserManager = idUserManager;
        }

        public Task<IdentityResult> CreateAsync(User user)
        {
            return idUserManager.CreateAsync(user);
        }

        public Task<User?> FindByPhoneNumberAsync(string phoneNumber)
        {
            return idUserManager.Users.SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public Task<User?> FindByUserNameAsync(string userName)
        {
            return idUserManager.FindByNameAsync(userName);
        }

        public Task<User?> FindByEmailAsync(string email)
        {
            return idUserManager.FindByEmailAsync(email);
        }
        
        public Task<User?> FindByUserAccountAsync(long userAccount)
        {
            return idUserManager.Users.FirstOrDefaultAsync(c => c.UserAccount == userAccount);
        }

        public Task<User?> FindByUserIdAsync(long userId)
        {
            return idUserManager.FindByIdAsync(userId.ToString());
        }

        public Task<User?> FindLastUserAccountAsync()
        {
            return idUserManager.Users.OrderBy(c => c.Id).LastOrDefaultAsync();
        }

        public Task<IdentityResult> UpdateAsync(User user)
        {
            return idUserManager.UpdateAsync(user);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return idUserManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                Role role = new Role { Name = roleName };
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    return result;
                }
            }
            return await idUserManager.AddToRoleAsync(user, roleName);
        }






        //public Task
    }
}
