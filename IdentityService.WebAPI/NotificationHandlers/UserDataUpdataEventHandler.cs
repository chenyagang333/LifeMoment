using Chen.EventBus;
using IdentityService.Domain.Notifications;
using MediatR;

namespace IdentityService.WebAPI.NotificationHandlers
{
    /// <summary>
    /// 更新用户信息 更新 用户的用户名 头像、
    /// </summary>
    public class UserDataUpdataEventHandler : INotificationHandler<UserDataUpdataEvent>
    {
        private readonly IEventBus eventBus;

        public UserDataUpdataEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }
        public Task Handle(UserDataUpdataEvent notification, CancellationToken cancellationToken)
        {
            string[] strs1 = ["UserName", "UserAvatar"];
            if (strs1.Contains(notification.Field))
            {
                eventBus.Publish("UserDataUpdata_UserName_UserAvatar", notification);
            }
            string[] strs2 = ["Description", "FansCount", "AttentionCount"];
            if (strs2.Contains(notification.Field))
            {
                eventBus.Publish("UserDataUpdata_Description_FansCount_AttentionCount", notification);
            }
            return Task.CompletedTask;
        }
    }

}
