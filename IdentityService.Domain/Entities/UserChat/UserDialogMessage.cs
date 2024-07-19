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
    /// 用户私聊 消息表
    /// </summary>
    public record UserDialogMessage
        (
            long UserDialogId,  // 用户会话ID // 索引
            long FromUserId,  // 发送者用户ID
            long ToUserId  // 接受者用户ID // 索引
        ) : BaseEntity, IHasCreateTime
    {
        public string? PostMessages { get; set; } // 发送消息
        public bool MarkRead { get; set; } = false; // 接收者_消息读取状态
        public bool RetractMessage { get; set; } = false; // 发送者_撤回消息
        public DateTime CreateTime { get; set; } = DateTime.Now; // 创建时间
        public bool FromUser_Deleted { get; set; } = false; // 发送者_删除
        public bool ToUser_Deleted { get; set; } = false; // 接收者_删除
        public MessagesType MessageType { get; set; } = MessagesType.Message; // 消息类型，默认为正常消息

        public UserDialogMessage RetractMessageHandler()
        {
            if (RetractMessage)
            {
                UpdatePostMessages($"撤回了一条信息。");
                MessageType = MessagesType.System;
            }
            return this;
        }
        public UserDialogMessage UpdatePostMessages(string msg)
        {
            PostMessages = msg;
            return this;
        }
        public UserDialogMessage UpdateMessageType(MessagesType messagesType)
        {
            MessageType = messagesType;
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
