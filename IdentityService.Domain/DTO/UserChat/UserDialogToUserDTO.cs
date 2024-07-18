using Chen.DomainCommons.Models;
using IdentityService.Domain.Entities.UserChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.DTO.UserChat
{
    public record UserDialogToUserDTO : UserDialogToUser, IHasModificationTime
    {
        public string LastMessage { get; set; } // 显示消息
        public DateTime? LastModificationTime { get; set; }
        public int UnreadCount { get; set; } // 未读信息数

        public UserDialogToUserDTO UpdateLastMessage(string msg)
        {
            LastMessage = msg;
            return this;
        }
        public UserDialogToUserDTO UpdateLastModificationTime(DateTime time)
        {
            LastModificationTime = time;
            return this;
        }
        public UserDialogToUserDTO UpdateUnreadCount(int count)
        {
            UnreadCount = count;
            return this;
        }
    }
}
