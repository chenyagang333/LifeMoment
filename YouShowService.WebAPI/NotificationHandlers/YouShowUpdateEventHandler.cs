using Chen.EventBus;
using MediatR;
using YouShowService.Domain.Notifications;

namespace YouShowService.WebAPI.NotificationHandlers
{
    /// <summary>
    /// 通知 SearchService 更新
    /// </summary>
    public class YouShowUpdateEventHandler : INotificationHandler<YouShowUpdateEvent>
    {
        private readonly IEventBus eventBus;

        public YouShowUpdateEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }
        public Task Handle(YouShowUpdateEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("ListeningStrength.Update", notification);
            return Task.CompletedTask;

        }
    }
}
