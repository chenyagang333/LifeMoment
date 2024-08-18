using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Entities.UserChat
{
    /// <summary>
    /// 用户私聊表 和 用户 关联表
    /// </summary>
    public record UserDialogToUser
        (
            long UserId,  // 用户ID 加索引 查询用
            long UserDialogId,  // 用户会话ID  UserId的复合索引
            long ToUserId,  // 对话 用户ID // 加索引，用来更新冗余数据
            string ToUserName, // 对话 用户名称 // 冗余
            string ToUserAvatar  // 对话 用户头像 // 冗余
        ) : BaseEntity, IHasCreateTime, IHasDeleteTime,ISoftDelete
    {

        public DateTime? TopTime { get; set; } = null; // 窗口置顶时间
        


        // 以下继承自接口


        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? DeletionTime { get; set; } // 删除对话时，再次打开对话，消息记录只显示删除时间以后的

        public bool IsDeleted { get; set; } = false;

        public UserDialogToUser UpdateDeletionTime()
        {
            DeletionTime = DateTime.Now;
            SoftDelete();
            return this;
        }

        public UserDialogToUser Top()
        {
            TopTime = DateTime.Now;
            return this;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
        }
        public UserDialogToUser SoftDelete(bool isDelete = true)
        {
            IsDeleted = isDelete;
            return this;
        }
    }
}
