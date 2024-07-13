using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.Entities
{
    public record Comment : BaseYouShowCommentReply
    {
        public long ShowId { get; init; }
        public int ReplyCount { get; private set; }

        public Comment AddReplyCount(int count)
        {
            ReplyCount += count;
            return this;
        }

        public Comment AddLikeCount(int count)
        {
            LikeCount += count;
            return this;
        }

    }
}
