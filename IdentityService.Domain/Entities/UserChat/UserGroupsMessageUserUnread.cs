using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities.UserChat
{
    /// <summary>
    /// 群聊消息和用户关联表,用户未读则添加一条记录，用户已读则不添加数据
    /// </summary>
    public record UserGroupsMessageUserUnread 
        (
        long UserGroupsId,// 用户群聊ID
        long ToUserId, // 接受者用户ID
        long UserGroupsMessageId// 消息ID
        )
    {
    }
}
