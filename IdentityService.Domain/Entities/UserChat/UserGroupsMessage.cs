using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities.UserChat
{
    /// <summary>
    /// 群聊消息表
    /// </summary>
    public record UserGroupsMessage: BaseEntity, IHasCreateTime
    {
        public DateTime CreateTime { get; set; } // 创建时间
        public long UserGroupsId { get; set; } // 用户群聊ID
        public long FromUserId { get; set; } // 发送者用户ID
        public string FromUserName { get; set; } // 发送者用户名 // 冗余
        public string FromUserAvatar { get; set; } // 发送者用户头像 // 冗余
        public string PostMessages { get; set; } // 发送消息
        public bool RetractMessage { get; set; } // 发送者_撤回消息
        public UserGroupsMessage RetractMessageHandler()
        {
            if (RetractMessage)
            {
                PostMessages = "";
            }
            return this;
        }
    }
}
