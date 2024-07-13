using Chen.DomainCommons.Models;
using IdentityService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    /// <summary>
    /// 用户通知消息
    /// </summary>
    public class Notify : IEntity
    {
        public Notify(
            DateTime occurrenceTime,
            long userId,
            string userName,
            string userAvatarURL,
            long locationId,
            YouShowNotifyType type
            )
        {
            
            OccurrenceTime = occurrenceTime;
            UserId = userId;
            UserName = userName;
            UserAvatarURL = userAvatarURL;
            LocationId = locationId;
            //Message = GetMessage(type);

        }
        public long Id { get; set; }
        public DateTime OccurrenceTime { get; init; }
        public long UserId { get; init; }
        public string UserName { get; init; }
        public string UserAvatarURL { get; init; }
        public long LocationId {  get; init; }
        public string Message {  get; init; }

        private (string? content,string? title) GetMessage(YouShowNotifyType type,string? message)
        {
            string? content;
            string? title;
            switch (type)
            {
                case YouShowNotifyType.LikeYouShow:
                    content = "赞了你的说说";
                    title = "";
                    break;
                case YouShowNotifyType.LikeComment:
                    content = "赞了你的评论";
                    title = "";
                    break;
                case YouShowNotifyType.LikeReply:
                    content = "赞了你的回复";
                    title = "";
                    break;
                case YouShowNotifyType.StarYouShow:
                    content = "收藏了你的说说";
                    title = "";
                    break;
                case YouShowNotifyType.CommentYouShow:
                    content = message;
                    title = "评论了你的说说";
                    break;
                case YouShowNotifyType.ReplyComment:
                    content = message;
                    title = "回复了你的评论";
                    break;
                case YouShowNotifyType.Attention:
                    content = "关注了你";
                    title = "";
                    break;
                default:
                    throw new NotImplementedException("找不到YouShowNotifyType的枚举类型");
            }
            return (content,title);
        }
    }
}
