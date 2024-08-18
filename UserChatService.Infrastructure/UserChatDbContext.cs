using Chen.Infrastructure.EFCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserChatService.Domain.Entities.UserChat;

namespace UserChatService.Infrastructure
{
    public class UserChatDbContext : BaseDbContext
    {
        public DbSet<UserDialog> UserDialogs { get; set; } // 用户私聊表
        public DbSet<UserDialogToUser> UserDialogToUsers { get; set; } // 用户私聊表 和 用户 关联表
        public DbSet<UserDialogMessage> UserDialogMessages { get; set; } // 用户私聊消息表
        public DbSet<UserGroups> UserGroups { get; set; } // 用户群聊表
        public DbSet<UserGroupsToUser> UserGroupsToUsers { get; set; } // 用户群聊表 和 用户关联表
        public DbSet<UserGroupsMessage> UserGroupsMessages { get; set; } // 用户群聊消息表
        public DbSet<UserGroupsMessageUserUnread> UserGroupsMessageUserUnreads { get; set; } // 用户未读群聊消息表
        public DbSet<UserGroupsMessageUserDeleted> UserGroupsMessageUserDeleteds { get; set; } // 用户删除群聊消息表

        public UserChatDbContext(DbContextOptions options, IMediator? mediator) : base(options, mediator)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
