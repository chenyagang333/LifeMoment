using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Entities.UserChat
{
    /// <summary>
    /// 群聊表 和 用户 关联表
    /// </summary>
    public record UserGroupsToUser
        (
        long UserGroupsId,  // 群聊Id 加索引 更新用
        long UserId,  // 用户Id 加索引 查询用  同时添加UserGroupsId的复合索引，用来查询删除时间
        string Name,  // 群名称 // 冗余
        string Icon  // 群图标 // 冗余
        ) : BaseEntity, IHasDeleteTime, ISoftDelete
    {
        public string UserNameWithInGroups { get; set; } // 群内 用户名
        public DateTime? TopTime { get; set; } // 窗口置顶时间
        public DateTime? DeletionTime { get; set; } // 删除对话时，再次打开对话，消息记录只显示删除时间以后的

        public bool IsDeleted { get; set; } = false;

        public void SoftDelete()
        {
            IsDeleted = true;
        }
        public UserGroupsToUser SoftDelete(bool isDelete = true)
        {
            IsDeleted = isDelete;
            return this;
        }

        public UserGroupsToUser UpdateDeletionTime()
        {
            DeletionTime = DateTime.Now;
            SoftDelete();
            return this;
        }
    }
}
