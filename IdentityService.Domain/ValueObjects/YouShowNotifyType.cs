using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.ValueObjects
{
    public enum YouShowNotifyType
    {
        /// <summary>
        /// 点赞 YouShow
        /// </summary>
        LikeYouShow,
        /// <summary>
        /// 点赞评论
        /// </summary>
        LikeComment,
        /// <summary>
        /// 点赞回复
        /// </summary>
        LikeReply,
        /// <summary>
        /// 收藏 YouShow
        /// </summary>
        StarYouShow,
        /// <summary>
        /// 评论 YouShow
        /// </summary>
        CommentYouShow,
        /// <summary>
        /// 回复评论
        /// </summary>
        ReplyComment,
        /// <summary>
        /// 点关注
        /// </summary>
        Attention,
    }
}
