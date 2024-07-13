using Chen.EventBus;
using MediatR;
using YouShowService.Domain.Notifications;

namespace YouShowService.WebAPI.NotificationHandlers
{
    /// <summary>
    /// 通知 SearchService 新增
    /// </summary>
    public class YouShowInsertEventHandler : INotificationHandler<YouShowInsertEvent>
    {
        private readonly IEventBus eventBus;

        public YouShowInsertEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public Task Handle(YouShowInsertEvent notification, CancellationToken cancellationToken)
        {
            var data = notification.Value;
            eventBus.Publish("ListeningStrength.Insert", data);
            return Task.CompletedTask;

        }
    }
}
