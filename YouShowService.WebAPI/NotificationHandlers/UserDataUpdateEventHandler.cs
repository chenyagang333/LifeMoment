using Chen.EventBus;
using MediatR;
using YouShowService.Domain.Notifications;

namespace YouShowService.WebAPI.NotificationHandlers
{
    /// <summary>
    /// 通知 IdentityService 更新数据
    /// </summary>
    public class UserDataUpdateEventHandler : INotificationHandler<UserDataUpdateEvent>
    {
        private readonly IEventBus eventBus;

        public UserDataUpdateEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }
        public Task Handle(UserDataUpdateEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("UserDataUpdate_FromYouShowToIdentity", notification);
            return Task.CompletedTask;
        }
    }
}
