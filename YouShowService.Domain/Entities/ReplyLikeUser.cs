using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.Entities
{
    public record ReplyLikeUser(long ReplyId, long UserId) : DomainEvents
    {
    }
}
