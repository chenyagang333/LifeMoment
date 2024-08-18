using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Entities.UserChat
{
    /// <summary>
    /// 群聊表
    /// </summary>
    public record UserGroups : BaseEntity, IHasCreateTime
    {
        public string Name { get; set; } // 群名称
        public DateTime CreateTime { get; set; } = DateTime.Now; // 创建时间
        public long AdminId { get; set; } // 群主Id
        public string Icon { get; set; } // 群图标
        public string Notice { get; set; } // 群公告
        public string Introduce { get; set; } // 群介绍

        public UserGroups UpdateName(string name)
        {
            Name = name;
            return this;
        }

        public UserGroups UpdateAdminId(long adminId)
        {
            AdminId = adminId;
            return this;
        }
        public UserGroups UpdateIcon(string icon)
        {
            Icon = icon;
            return this;
        }

        public UserGroups UpdateNotice(string notice)
        {
            Notice = notice;
            return this;
        }
        public UserGroups UpdateIntroduce(string introduce)
        {
            Introduce = introduce;
            return this;
        }

    }
}
