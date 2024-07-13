using Chen.EventBus;
using IdentityService.Domain.Notifications;
using MediatR;

namespace IdentityService.WebAPI.NotificationHandlers
{
    public class UserDataInsertEventHandler : INotificationHandler<AddUserEvent>
    {
        private readonly IEventBus eventBus;

        public UserDataInsertEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }
        public Task Handle(AddUserEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("UserDataInsert", notification);
            return Task.CompletedTask;
        }
    }

}
