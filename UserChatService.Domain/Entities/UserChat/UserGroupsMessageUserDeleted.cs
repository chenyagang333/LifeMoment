using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Entities.UserChat
{
    /// <summary>
    /// 用户删除群聊消息记录表，存在记录则用户删除了该信息
    /// </summary>
    /// <param name="UserGroupsMessageId"></param>
    /// <param name="UserId"></param>
    public record UserGroupsMessageUserDeleted
        (
        long UserGroupsId,// 用户群聊ID
        long ToUserId, // 接受者用户ID
        long UserGroupsMessageId // 消息ID
        )
    {
    }
}
