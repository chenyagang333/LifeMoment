using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Model.Request
{
    public class CreateUserGroupsEntity
    {
        public long admainId { get; set; }
        public string icon { get; set; }
        public IEnumerable<CreateUserGroupsToUser> CreateUserGroupsToUsers { get; set; }
    }
    public class CreateUserGroupsToUser
    {
        public long userId { get; set; }
        public string userName { get; set; }
    }
}
