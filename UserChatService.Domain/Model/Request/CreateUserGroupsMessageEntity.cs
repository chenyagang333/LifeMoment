using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Model.Request
{
    public class CreateUserGroupsMessageEntity
    {
        public long UserGroupsId { get; set; }
        public long FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string FromUserAvatar { get; set; }
        public string PostMessagese { get; set; }
        public IEnumerable<long> ReceiveUserIds { get; set; }
    }
}
