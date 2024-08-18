using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserChatService.Domain.Entities.UserChat;
using UserChatService.Domain.IService;
using UserChatService.Domain.Model.Request;
using UserChatService.Domain.Model.Response;
using UserChatService.Infrastructure.Hubs;
using Chen.Commons;

namespace UserChatService.Infrastructure.Service;

public class UserChatService : IUserChatService
{
    private readonly IUserDialogService userDialogService;
    private readonly IUserGroupsService userGroupsService;

    public UserChatService(IUserDialogService userDialogService,IUserGroupsService userGroupsService)
    {
        this.userDialogService = userDialogService;
        this.userGroupsService = userGroupsService;
    }

    // 获取会话列表
    public async Task<IEnumerable<dynamic>> GetDialogAndGroupsByUserIdAsync(long userId)
    {
        var list1 = await userDialogService.GetDialogByUserIdAsync(userId);
        var list2 = await userGroupsService.GetGroupsByUserIdAsync(userId);
        // 合并
        List<dynamic> dynamics = [.. list1, .. list2];
        return dynamics.OrderByDescending(x => x.TopTime).ThenByDescending(x => x.LastModificationTime);
    }




}
