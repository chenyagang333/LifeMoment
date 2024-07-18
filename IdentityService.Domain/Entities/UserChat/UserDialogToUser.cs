using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities.UserChat
{
    /// <summary>
    /// 用户私聊表 和 用户 关联表
    /// </summary>
    public record UserDialogToUser : BaseEntity, ISoftDelete, IHasCreateTime, IHasDeleteTime
    {
        public long UserId { get; set; } // 用户ID 加索引 查询用
        public long ToUserId { get; set; } // 对话 用户ID
        public string ToUserName { get; set; } // 对话 用户名称 // 冗余
        public string ToUserAvatar { get; set; } // 对话 用户头像 // 冗余
        public long UserDialogId { get; set; } // 用户会话ID  加索引 更新用
        public DateTime? TopTime { get; set; } = null; // 窗口置顶时间
        


        // 以下继承自接口

        public bool IsDeleted { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? DeletionTime { get; set; }


        public void SoftDelete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.Now;
        }

        public void Top()
        {
            TopTime = DateTime.Now;
        }




    }
}
