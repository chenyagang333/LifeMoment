using Chen.DomainCommons.Models;
using UserChatService.Domain.Entities.UserChat;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Model.Response
{
    public record UserDialogToUserDTO : UserDialogToUser
    {
        public UserDialogToUserDTO() : base(0, 0, 0, "", "")
        {

        }
        public string LastMessage { get; set; } // 显示消息
        public DateTime? LastPostMessageTime { get; set; }
        public int UnreadCount { get; set; } // 未读信息数

        public UserDialogToUserDTO UpdateLastMessage(string? msg)
        {
            LastMessage = msg;
            return this;
        }
        public UserDialogToUserDTO UpdateLastPostMessageTime(DateTime time)
        {
            LastPostMessageTime = time;
            return this;
        }
        public UserDialogToUserDTO UpdateUnreadCount(int count)
        {
            UnreadCount = count;
            return this;
        }
    }
}
