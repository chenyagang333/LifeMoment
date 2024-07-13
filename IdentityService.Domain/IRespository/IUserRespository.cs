 using IdentityService.Domain.Entities;
using IdentityService.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.IRespository
{
    public interface IUserRespository
    {
        Task<User?> FindByPhoneNumberAsync(string phoneNumber);
        Task<User?> FindByUserNameAsync(string phoneNumber);
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByUserIdAsync(long userId);
        Task<User?> FindByUserAccountAsync(long userAccount);
        Task<User?> FindLastUserAccountAsync();
        Task<IdentityResult> UpdateAsync(User user);
        Task<IdentityResult> CreateAsync(User user);

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<string>> GetRolesAsync(User user);

        /// <summary>
        /// 为用户 user 添加角色 roleName
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<IdentityResult> AddToRoleAsync(User user,string roleName);   

    }
}
