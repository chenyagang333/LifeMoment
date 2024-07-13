using Chen.EventBus;
using IdentityService.Domain.Notifications;
using IdentityService.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.WebAPI.EventHandlers
{
    [EventName("UserDataUpdate_FromYouShowToIdentity")]
    public class ListeningUserDataUpdateEventHandler : JsonIntegrationEventHandler<UserDataUpdateEvent>
    {
        private readonly UserDbContext ctx;
        private readonly IEventBus eventBus;

        public ListeningUserDataUpdateEventHandler(UserDbContext ctx, IEventBus eventBus)
        {
            this.ctx = ctx;
            this.eventBus = eventBus;
        }


        public override async Task HandleJson(string eventName, UserDataUpdateEvent? eventData)
        {
            if (eventData != null)
            {
                var user = await ctx.Users.FirstAsync(x => x.Id == eventData.UserId);
                if (eventData.type == UserDataUpdateEventType.UpdateLikeCount)
                {
                    user.AddLikeCount(eventData.addCount);
                }
                else if (eventData.type == UserDataUpdateEventType.UpdateStarCount)
                {
                    user.AddStarCount(eventData.addCount);
                }
                else if (eventData.type == UserDataUpdateEventType.UpdateContentCount)
                {
                    user.AddContentCount(eventData.addCount);
                }
                else if (eventData.type == UserDataUpdateEventType.UpdateGetLikeCount)
                {
                    user.AddGetLikeCount(eventData.addCount);
                }
                await ctx.SaveChangesAsync();
                // 更新成功后，通知 SearchService 更细数据
                eventBus.Publish("UserDataUpdate_GetLikeCount",
                    new UserDataUpdataEvent(user.Id, "GetLikeCount", user.GetLikeCount));
            }
        }
    }

    public record UserDataUpdateEvent(long UserId, int addCount, UserDataUpdateEventType type);

    public enum UserDataUpdateEventType
    {
        UpdateLikeCount,
        UpdateStarCount,
        UpdateContentCount,
        UpdateGetLikeCount,
    }
}
