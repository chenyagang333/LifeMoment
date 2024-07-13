using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.Entities
{
    public record Reply : BaseYouShowCommentReply
    {
        public long CommentId { get; init; }    
        public string? ToUserName { get; private set; }

        public Reply ChangeToUserName(string toUserName)
        {
            ToUserName = toUserName;
            return this;
        }
        public Reply AddLikeCount(int count)
        {
            LikeCount += count;
            return this;
        }
    }
}
