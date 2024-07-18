using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities.UserChat
{
    /// <summary>
    /// 群聊表 和 用户 关联表
    /// </summary>
    public record UserGroupsToUser: BaseEntity
    {
        public long UserId { get; set; } // 用户Id 加索引 查询用
        public long UserGroupsId { get; set; } // 群聊Id 加索引 更新用
        public string Name { get; set; } // 群名称 // 冗余
        public string Icon { get; set; } // 群图标 // 冗余
        public string UserNameWithInGroups { get; set; } // 群内 用户名
        public DateTime? TopTime { get; set; } // 窗口置顶时间

    }
}
