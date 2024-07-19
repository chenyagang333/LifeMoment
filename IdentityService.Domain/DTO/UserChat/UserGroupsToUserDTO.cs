using Chen.DomainCommons.Models;
using IdentityService.Domain.Entities.UserChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.DTO.UserChat
{
    public record UserGroupsToUserDTO : UserGroupsToUser
    {
        public UserGroupsToUserDTO() : base(0, 0, "", "")
        {

        }
        public string LastMessage { get; set; } // 显示消息
        public DateTime? LastPostMessageTime { get; set; }
        public int UnreadCount { get; set; } // 未读信息数

        public UserGroupsToUserDTO UpdateLastMessage(string msg)
        {
            LastMessage = msg;
            return this;
        }
        public UserGroupsToUserDTO UpdateLastPostMessageTime(DateTime time)
        {
            LastPostMessageTime = time;
            return this;
        }
        public UserGroupsToUserDTO UpdateUnreadCount(int count)
        {
            UnreadCount = count;
            return this;
        }
    }
}
