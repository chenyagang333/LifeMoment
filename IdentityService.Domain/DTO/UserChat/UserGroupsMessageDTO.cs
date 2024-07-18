using IdentityService.Domain.Entities.UserChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.DTO.UserChat
{
    public record UserGroupsMessageDTO : UserGroupsMessage
    {
        public bool Deleted { get; set; } // 接收者_删除

        public UserGroupsMessageDTO IsDeleted(bool deleted = true)
        {
            Deleted = deleted;
            return this;
        }


    }
}
