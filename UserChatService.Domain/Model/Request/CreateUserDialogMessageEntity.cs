using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Model.Request
{
    public class CreateUserDialogMessageEntity
    {
        public long userDialogId { get; set; }
        public long userId { get; set; }
        public long toUserId { get; set; }
        public string? message { get; set; }
    }
}
