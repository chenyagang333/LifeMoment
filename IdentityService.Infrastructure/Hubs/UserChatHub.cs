using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.UserChat;
using IdentityService.Domain.IService;
using IdentityService.Domain.ServiceEntities.UserChat;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Hubs
{
    public class UserChatHub : Hub
    {
        private readonly IUserChat userChat;

        public UserChatHub(IUserChat userChat)
        {
            this.userChat = userChat;
        }

        #region 群聊或私聊列表模块

        // 私聊窗口创建
        //public async Task CreateUserDialog(CreateUserDialogEntity e)
        //{
        //    // 1.创建私聊窗口
        //    var dialogId = await userChat.CreateUserDialog(e);
        //    int StatusCode = 200;
        //    if (dialogId > 0)
        //    {
        //        // 对方的对话框显示发起聊天用户的信息
        //        var toUserDialog = new UserDialogToUser(e.toUserId, dialogId, e.userId, e.userName, e.userAvatar);
        //        await Clients.User(e.toUserId.ToString()).SendAsync("CreateUserDialog", toUserDialog);
        //    }
        //    Groups.AddToGroupAsync();
        //    else
        //    {
        //        StatusCode = 400;
        //    }
        //    // 返回对话框的创建状态
        //    await Clients.User(e.userId.ToString()).SendAsync("CreateUserDialogResult", StatusCode); // 
        //    await Groups.AddToGroupAsync(Context.ConnectionId, "dsa");
        //}

        #endregion

    }
}
