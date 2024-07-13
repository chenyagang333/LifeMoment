using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    /// <summary>
    /// 新用户随机头像（相对路径）
    /// </summary>
    public record RandomUserAvatar: BaseEntity
    {
        /// <summary>
        /// （相对路径）
        /// </summary>
        public string AvatarUrl { get; set; }
    }
}
