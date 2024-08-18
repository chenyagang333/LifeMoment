using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Entities.UserChat
{
    public record UserGroupsMessageUserUnread
        (
        long UserGroupsId,// 用户群聊ID
        long ToUserId, // 接受者用户ID
        long UserGroupsMessageId // 消息ID
        )
    {
    }
}
