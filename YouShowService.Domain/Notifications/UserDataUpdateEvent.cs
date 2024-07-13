using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.Notifications
{
    /// <summary>
    /// 通知 IdentityService 更新数据
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="addCount"></param>
    /// <param name="type"></param>
    public record UserDataUpdateEvent(long UserId, int addCount, UserDataUpdateEventType type) : INotification;

    public enum UserDataUpdateEventType
    {
        UpdateLikeCount,
        UpdateStarCount,
        UpdateContentCount,
        UpdateGetLikeCount,
    }
}
