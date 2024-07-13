using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Notifications;

namespace YouShowService.Domain.Entities
{
    public record YouShow : BaseYouShowCommentReply
    {
        public string? LikeUsers { get;  set; }
        public int ViewCount { get;  set; }
        public int StarCount { get;  set; }
        public int ShareCount { get;  set; }
        public int CommentCount { get;  set; }

        public YouShow Build()
        {
            AddDomainEventIfAbsent(new YouShowInsertEvent(this));
            return this;
        }
        public YouShow Delete()
        {
            AddDomainEventIfAbsent(new YouShowDeleteEvent(Id));
            return this;
        }

        public YouShow AddUserYouShowCount(int addCount)
        {
            AddDomainEventIfAbsent(new UserDataUpdateEvent
              (UserId, addCount, UserDataUpdateEventType.UpdateContentCount));
            return this;
        }

        public YouShow UpdateLikeUsers(string likeUsers)
        {
            LikeUsers = likeUsers;
            AddDomainEventIfAbsent(new YouShowUpdateEvent
                (Id, likeUsers,YouShowUpdateEventType.UpdateLikeUsers));
            return this;
        }
        public YouShow AddViewCount(int viewCount)
        {
            ViewCount += viewCount;
            AddDomainEventIfAbsent(new YouShowUpdateEvent
                (Id, ViewCount, YouShowUpdateEventType.AddViewCount));
            return this;
        }
        public YouShow AddShareCount(int shareCount)
        {
            ShareCount += shareCount;
            AddDomainEventIfAbsent(new YouShowUpdateEvent
                (Id, ShareCount, YouShowUpdateEventType.AddShareCount));
            return this;
        }
        /// <summary>
        /// 更新评论数量
        /// increment：增量
        /// </summary>
        /// <param name="increment">增量</param>
        /// <returns></returns>
        public YouShow AddCommentCount(int increment)
        {
            CommentCount += increment;
            AddDomainEventIfAbsent(new YouShowUpdateEvent
                (Id, CommentCount, YouShowUpdateEventType.AddCommentCount));
            return this;
        }

        public YouShow AddLikeCount(int count)
        {
            LikeCount += count;
            AddDomainEventIfAbsent(new YouShowUpdateEvent
                (Id, LikeCount, YouShowUpdateEventType.AddLikeCount));
            AddDomainEventIfAbsent(new UserDataUpdateEvent(UserId,count,UserDataUpdateEventType.UpdateGetLikeCount)); // 更新用户总获赞
            return this;
        }

        public YouShow AddStarCount(int count)
        {
            StarCount += count;
            AddDomainEventIfAbsent(new YouShowUpdateEvent
                (Id, StarCount, YouShowUpdateEventType.AddStarCount));
            return this;
        }


    }
}
