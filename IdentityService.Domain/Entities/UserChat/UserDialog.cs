using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities.UserChat
{
    /// <summary>
    /// 用户对话表（私聊表）
    /// </summary>
    public record UserDialog() : DomainEvents
    {
        public long Id { get; set; }
    }
}
