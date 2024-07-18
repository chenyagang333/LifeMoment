using Chen.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using YouShowService.Domain.Entities;
using YouShowService.Infrastructure;

namespace YouShowService.WebAPI.EventHandlers
{
    [EventName("UserDataUpdata_UserName_UserAvatar")]
    public class ListeningYouShowUpdateEventHandler : JsonIntegrationEventHandler<UserDataUpdateEvent>
    {
        private readonly YouShowDbContext ctx;

        public ListeningYouShowUpdateEventHandler(YouShowDbContext ctx)
        {
            this.ctx = ctx;
        }

        public override async Task HandleJson(string eventName, UserDataUpdateEvent? eventData)
        {
            if (eventData != null)
            {
                string value = eventData.Value.ToString();
                if (value != null)
                {
                    if (eventData.Field == "UserName")
                    {
                        await ctx.YouShows.Where(x => x.UserId == eventData.UserId)
                            .ExecuteUpdateAsync(s => s.SetProperty(e => e.UserName, e => value));
                    }
                    else if (eventData.Field == "UserAvatar")
                    {
                        await ctx.YouShows.Where(x => x.UserId == eventData.UserId)
                            .ExecuteUpdateAsync(s => s.SetProperty(e => e.UserAvatarURL, e => value));
                    }
                }
            }
        }


    }
    public record UserDataUpdateEvent(long UserId, string Field, dynamic Value);
}
