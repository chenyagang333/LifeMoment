using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserChatService.Domain.IService;
using UserChatService.Infrastructure.Hubs;

namespace UserChatService.Infrastructure.Service
{
    public class UserHubService : IUserHubService
    {
        private readonly IHubContext<UserChatHub> hubContext;
        public UserHubService(IHubContext<UserChatHub> hubContext)
        {
            this.hubContext = hubContext;

        }
        public Task SendDataByUserIdAsync(long userId, string method, object data)
        {
            return hubContext.Clients.User(userId.ToString()).SendAsync(method, data);
        }
        //
        public Task SendDataByUserIdAsync(IEnumerable<long> userIds, string method, object data)
        {
            return hubContext.Clients.Users(userIds.Select(x => x.ToString())).SendAsync(method, data);
        }
    }
}
