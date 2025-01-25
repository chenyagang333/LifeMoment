using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Notifications;

namespace YouShowService.Domain.Entities
{
    public record YouShowLikeUser : YouShowUserBase
    {
        public YouShowLikeUser(long YouShowId, long UserId) :base(YouShowId, UserId)
        {
            
        }
        public YouShowLikeUser AddUserLikeCount(int addCount)
        {
            AddDomainEventIfAbsent(new UserDataUpdateEvent
                (UserId, addCount, UserDataUpdateEventType.UpdateLikeCount));
            return this;
        }
    }
}
