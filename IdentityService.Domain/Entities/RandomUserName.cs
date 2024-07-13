using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    /// <summary>
    /// 新用户随机昵称
    /// </summary>
    public record RandomUserName: BaseEntity
    {
        public string UserName { get; set; }
    }
}
