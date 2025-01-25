using Chen.DomainCommons.Models;
using YouShowService.Domain.Notifications;

namespace YouShowService.Domain.Entities
{
    public record YouShowStarUser : YouShowUserBase
    {
        public YouShowStarUser(long YouShowId, long UserId) : base(YouShowId, UserId)
        {

        }
        public YouShowStarUser AddUserStarCount(int addCount)
        {
            AddDomainEventIfAbsent(new UserDataUpdateEvent
                (UserId, addCount, UserDataUpdateEventType.UpdateStarCount));
            return this;
        }
    }
}
