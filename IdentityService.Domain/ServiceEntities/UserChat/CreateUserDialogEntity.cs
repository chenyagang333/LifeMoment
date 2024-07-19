using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.ServiceEntities.UserChat
{
    public class CreateUserDialogEntity
    {
        public long userId { get; set; }
        public string userName { get; set; }
        public string userAvatar { get; set; }
        public long toUserId { get; set; }
        public string toUserName { get; set; }
        public string toUserAvatar { get; set; }
    }
}
