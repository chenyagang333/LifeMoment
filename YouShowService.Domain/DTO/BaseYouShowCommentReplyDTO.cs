using Chen.Commons.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YouShowService.Domain.DTO
{
    public record BaseYouShowCommentReplyDTO
    {
        public long UserId { get; private set; }
        public string? UserName { get; private set; }
        public string? UserAvatarURL { get; private set; }
        public string? PublishAddress { get; private set; }
        public string? Content { get; private set; }
        public int LikeCount { get; private set; }

        public DateTime CreateTime { get; private set; }
        public long Id { get; protected set; }


    }
}
