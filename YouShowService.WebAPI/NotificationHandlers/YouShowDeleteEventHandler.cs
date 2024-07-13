using Chen.EventBus;
using MediatR;
using YouShowService.Domain.Notifications;

namespace YouShowService.WebAPI.NotificationHandlers
{
    /// <summary>
    /// 通知 SearchService删除
    /// </summary>
    public class YouShowDeleteEventHandler : INotificationHandler<YouShowDeleteEvent>
    {
        private readonly IEventBus eventBus;

        public YouShowDeleteEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }
        public Task Handle(YouShowDeleteEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("ListeningStrength.Hidden", notification.YoushowId);
            return Task.CompletedTask;
        }
    }
}
