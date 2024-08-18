using UserChatService.Domain.Entities.UserChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Model.Response
{
    public record UserGroupsMessageDTO : UserGroupsMessage
    {
        public UserGroupsMessageDTO() : base(0, 0, "", "", "")
        {

        }

        public bool Unread { get; set; } = true; // 查询用户，未读
        public bool Deleted { get; set; } // 接收者_删除

        public UserGroupsMessageDTO IsDeleted(bool deleted = true)
        {
            Deleted = deleted;
            return this;
        }
        public UserGroupsMessageDTO IsDeleted(IEnumerable<long> deletedIds)
        {
            if (deletedIds.Contains(Id)) { Deleted = true; }
            return this;
        }

        public UserGroupsMessageDTO IsUnread(bool unread = true)
        {
            Unread = unread;
            return this;
        }

        public UserGroupsMessageDTO IsUnread(IEnumerable<long> unreadIds)
        {
            if (unreadIds.Contains(Id)) { Unread = true; }
            return this;
        }


    }
}
