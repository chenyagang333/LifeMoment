using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities.UserChat
{
    /// <summary>
    /// 用户私聊 消息表
    /// </summary>
    public record UserDialogMessage : BaseEntity, IHasCreateTime
    {
        public long UserDialogId { get; set; } // 用户会话ID // 索引
        public long FromUserId { get; set; } // 发送者用户ID
        public long ToUserId { get; set; } // 接受者用户ID // 索引
        public string PostMessages { get; set; } // 发送消息
        public bool Received { get; set; } = false; // 接收者_接收状态
        public bool RetractMessage { get; set; } = false; // 发送者_撤回消息
        public DateTime CreateTime { get; set; } = DateTime.Now; // 创建时间
        public bool FromUser_Deleted { get; set; } = false; // 发送者_删除
        public bool ToUser_Deleted { get; set; } = false; // 接收者_删除

        public UserDialogMessage RetractMessageHandler()
        {
            if (RetractMessage)
            {
                PostMessages = "";
            }
            return this;
        }

        public UserDialogMessage DeletedHandler(long userId)
        {
            if (userId == FromUserId)
            {
                FromUser_Deleted = true;
            }
            else if (userId == ToUserId)
            {
                ToUser_Deleted = true;
            }
            return this;
        }

    }
}
