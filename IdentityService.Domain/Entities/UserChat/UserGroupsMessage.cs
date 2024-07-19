using Chen.DomainCommons.Models;
using IdentityService.Domain.Enums.UserChat;
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
    public record UserGroupsMessage
        (
        long UserGroupsId,  // 用户群聊ID
        long FromUserId,  // 发送者用户ID
        string FromUserName,  // 发送者用户名 // 冗余
        string FromUserAvatar,  // 发送者用户头像 // 冗余
        string PostMessages  // 发送者_撤回消息
        ) : BaseEntity, IHasCreateTime
    {
        public DateTime CreateTime { get; set; } = DateTime.Now; // 创建时间
        public string PostMessages { get; set; } = PostMessages; // 发送消息
        public bool RetractMessage { get; set; } // 发送者_撤回消息
        public MessagesType MessageType { get; set; } = MessagesType.Message; // 消息类型，默认为正常消息

        public UserGroupsMessage RetractMessageHandler()
        {
            if (RetractMessage)
            {
                UpdatePostMessages($"{FromUserName}撤回了一条信息。");
                MessageType = MessagesType.System;
            }
            return this;
        }

        public UserGroupsMessage UpdatePostMessages(string nsg)
        {
            PostMessages = nsg;
            return this;
        }

        public UserGroupsMessage UpdateMessageType(MessagesType messagesType)
        {
            MessageType = messagesType;
            return this;
        }
    }
}
