using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.Notifications
{
    /// <summary>
    /// 文章的数据变动时，通知 SearchService更新数据
    /// </summary>
    /// <param name="YouShowId"></param>
    /// <param name="UpdateData"></param>
    /// <param name="type"></param>
    public record YouShowUpdateEvent(long YouShowId,dynamic UpdateData, YouShowUpdateEventType type) : INotification;

    public enum YouShowUpdateEventType
    {
        UpdateLikeUsers,
        AddViewCount,
        AddShareCount,
        AddCommentCount,
        AddLikeCount,
        AddStarCount,
    }
}
