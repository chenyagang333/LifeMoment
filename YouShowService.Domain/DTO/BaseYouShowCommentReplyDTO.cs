using Chen.Commons.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.DTO
{
    public record BaseYouShowCommentReplyDTO: BaseYouShowCommentReply
    {
        public BaseYouShowCommentReplyDTO SpliceUserAvatarURL(string baseUrl)
        {
            if (!string.IsNullOrEmpty(UserAvatarURL))
            {
                UserAvatarURL = baseUrl + UserAvatarURL;
            }
            return this;
        }
    }
}
