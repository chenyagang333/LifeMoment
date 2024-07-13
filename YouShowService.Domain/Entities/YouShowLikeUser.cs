using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Notifications;

namespace YouShowService.Domain.Entities
{
    public record YouShowLikeUser(long YouShowId, long UserId) : DomainEvents
    {
        public long Id { get; set; }
        public YouShowLikeUser AddUserLikeCount(int addCount)
        {
            AddDomainEventIfAbsent(new UserDataUpdateEvent
                (UserId, addCount, UserDataUpdateEventType.UpdateLikeCount));
            return this;
        }
    }
}
