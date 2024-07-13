using Chen.DomainCommons.Models;
using YouShowService.Domain.Notifications;

namespace YouShowService.Domain.Entities
{
    public record YouShowStarUser(long YouShowId, long UserId) : DomainEvents
    {
        public long Id { get; set; }
        public YouShowStarUser AddUserStarCount(int addCount)
        {
            AddDomainEventIfAbsent(new UserDataUpdateEvent
                (UserId, addCount, UserDataUpdateEventType.UpdateStarCount));
            return this;
        }
    }
}
