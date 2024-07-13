using Chen.Infrastructure.EFCore;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class UserDbContext:IdentityDbContext<User,Role,long>
    {
        public DbSet<User> Users { get; set; } // 用户表
        public DbSet<Role> Roles { get; set; } // 用户权限
        public DbSet<SignIn> SignIns { get; set; } // 用户签到
        public DbSet<RandomUserAvatar> RandomUserAvatars { get; set; } // 新用户随机头像
        public DbSet<RandomUserName> RandomUserNames { get; set; } // 新用户随机昵称
        public DbSet<UserAttentionUser> UserAttentionUsers { get; set; } // 用户关注 和用户粉丝的关系
        public UserDbContext(DbContextOptions<UserDbContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            builder.EnableSoftDeletionGlobalFilter(); // 添加软删除的过滤条件
        }
    }
}
