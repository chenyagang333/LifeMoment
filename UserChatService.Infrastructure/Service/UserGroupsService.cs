using AutoMapper;
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
using Chen.Commons;

namespace UserChatService.Infrastructure.Service;

public class UserGroupsService : IUserGroupsService
{
    private readonly UserChatDbContext context;
    private readonly IMapper mapper;
    private readonly IUserHubService userHubService;

    public UserGroupsService(UserChatDbContext context, IMapper mapper, IUserHubService userHubService)
    {
        this.context = context;
        this.mapper = mapper;
        this.userHubService = userHubService;
    }

    // 创建群聊
    public async Task<long> CreateUserGroupsAsync(CreateUserGroupsEntity e)
    {
        var names = e.CreateUserGroupsToUsers.Select(x => x.userName);
        var name = string.Join("、", names);
        var userGroups = new UserGroups().UpdateName(name).UpdateAdminId(e.admainId).UpdateIcon(e.icon);
        try
        {
            using var transaction = context.Database.BeginTransaction();
            await context.UserGroups.AddAsync(userGroups);
            await context.SaveChangesAsync(); // 拿到群聊Id
            List<UserGroupsToUser> userGroupsToUsers = new List<UserGroupsToUser>();
            foreach (var item in e.CreateUserGroupsToUsers)
            {
                UserGroupsToUser entity = new(userGroups.Id, item.userId, name, e.icon);
                userGroupsToUsers.Add(entity);
            }
            // 批量插入 UserGroupsToUser
            await context.BulkInsertAsync(userGroupsToUsers);
            await context.SaveChangesAsync();
            transaction.Commit();
        }
        catch (Exception)
        {
            return 0;
        }
        var receiveUserIds = e.CreateUserGroupsToUsers.Select(x => x.userId).Where(x => x != e.admainId);
        await userHubService.SendDataByUserIdAsync(receiveUserIds, "CreateUserGroups", userGroups);
        // 新建群聊提示信息
        await CreateGroupsMessageAsync(new CreateUserGroupsMessageEntity
        {
            UserGroupsId = userGroups.Id,
            FromUserId = 0,
            FromUserName = "System",
            FromUserAvatar = "",
            PostMessagese = $"{names} 加入了群聊",
            ReceiveUserIds = receiveUserIds
        });
        return userGroups.Id;
    }
    // 获取群聊会话列表
    public async Task<IEnumerable<UserGroupsToUserDTO>> GetGroupsByUserIdAsync(long userId)
    {
        // 群聊表
        var userGroupsToUsers = await context.UserGroupsToUsers.Where(x => x.UserId == userId).ToListAsync();
        // 映射DTO
        var userGroupsToUsersDTO = mapper.Map<List<UserGroupsToUser>, List<UserGroupsToUserDTO>>(userGroupsToUsers);
        // 拿到群聊表ID数组
        var userGroupsIds = userGroupsToUsers.Select(x => x.UserGroupsId);
        // 查询最后一条信息和时间
        var LastDatas = await context.UserGroupsMessages
                .Where(x => userGroupsIds.Contains(x.UserGroupsId))
                .GroupBy(x => x.UserGroupsId)
                .Select(x => new
                {
                    UserGroupsId = x.Key,
                    LastData = x.OrderByDescending(o => o.Id)
                                    .Select(s => new { s.PostMessages, s.CreateTime }).First()
                }).ToListAsync();
        // 拿到该群聊未读信息数
        var UnreadCounts = await context.UserGroupsMessageUserUnreads
                                .Where(x => userGroupsIds.Contains(x.UserGroupsId) && x.ToUserId == userId)
                                .GroupBy(x => x.UserGroupsId)
                                .Select(x => new
                                {
                                    UserGroupsId = x.Key,
                                    UnreadCount = x.Count()
                                }).ToListAsync();
        // 为DTO 赋值
        userGroupsToUsersDTO.ForEach(item =>
        {
            var item1 = LastDatas.First(x => item.UserGroupsId == x.UserGroupsId);
            item.UpdateLastMessage(item1.LastData.PostMessages).UpdateLastPostMessageTime(item1.LastData.CreateTime);
            var item2 = UnreadCounts.First(x => item.UserGroupsId == x.UserGroupsId);
            item.UpdateUnreadCount(item2.UnreadCount);
        });
        // 筛选未删除的对话框
        return userGroupsToUsersDTO.Where(x => x.LastPostMessageTime > x.DeletionTime || !x.IsDeleted);
    }
    // 更新群聊表冗余的群名称、群图像
    public async Task UpdateUserGroupsToUserAsync(long userGroupsId, string name, string icon)
    {
        await context.UserGroupsToUsers.Where(x => x.UserGroupsId == userGroupsId)
        .ExecuteUpdateAsync(s => s
        .SetProperty(e => e.Name, e => name)
        .SetProperty(e => e.Icon, e => icon)
        );
    }
    // 获取群聊会话信息
    public async Task<(IEnumerable<UserGroupsMessageDTO> list, bool over)> GetUserGroupsMessageByUserGroupsIdAsync(long userId, long userGroupsId, int pageSize, long beginId = 0)
    {
        bool over = false; // 提示前端查询是否结束
        var baseQuery = context.UserGroupsMessages.Where(x => x.UserGroupsId == userGroupsId);
        if (beginId > 0)
        {
            baseQuery = context.UserGroupsMessages.Where(x => x.UserGroupsId == userGroupsId && x.Id < beginId);
        }
        // 拿到消息
        var userGroupsMessages = await baseQuery.OrderByDescending(x => x.Id).Paging(pageSize, 1).ToListAsync();
        var userGroupsMessagesDTO = mapper.Map<List<UserGroupsMessage>, List<UserGroupsMessageDTO>>(userGroupsMessages);
        // 拿到所有信息的Id
        var userGroupsMessagesIds = userGroupsMessages.Select(x => x.Id);

        // 该用户删除的消息的ID集合
        var deleteIds = await context.UserGroupsMessageUserDeleteds
            .Where(x => x.UserGroupsId == userGroupsId &&
                        x.ToUserId == userId &&
                        userGroupsMessagesIds.Contains(x.UserGroupsMessageId))
            .Select(x => x.UserGroupsMessageId).ToListAsync();

        // 该用户未读的消息Id集合
        var unreadIds = await context.UserGroupsMessageUserUnreads
            .Where(x => x.UserGroupsId == userGroupsId &&
                        x.ToUserId == userId &&
                        userGroupsMessagesIds.Contains(x.UserGroupsMessageId))
            .Select(x => x.UserGroupsMessageId).ToListAsync();

        // 更新每条信息的读取状态、删除状态、撤回状态
        userGroupsMessagesDTO.ForEach(x => x.IsDeleted(deleteIds).IsUnread(unreadIds).RetractMessageHandler());
        IEnumerable<UserGroupsMessageDTO> result = userGroupsMessagesDTO;
        // 防止用户连续删除数据数量大于等于 pageSize 导致查该页数据为0
        if (result.Count() > 0 && result.All(x => x.Deleted))
        {
            (result, over) = await GetUserGroupsMessageByUserGroupsIdAsync(userId, userGroupsId, pageSize, beginId);
        }
        // 筛除该用户已删除的数据，如果
        result = userGroupsMessagesDTO.Where(x => !x.Deleted);
        // 查询该对话框用户的删除时间
        var data = await context.UserGroupsToUsers.SingleAsync(x => x.UserId == userId && x.UserGroupsId == userGroupsId);
        if (result.Any(x => x.CreateTime < data.DeletionTime))
        {
            result = result.Where(x => x.CreateTime > data.DeletionTime);
            over = true;
        }
        if (result.Count() == 0) { over = true; }
        return (result, over);
    }
    // 更新群聊 消息 表冗余的用户名、用户图像
    public async Task UpdateUserGroupsMessageAsync(long fromUserId, string fromUserName, string fromUserAvatar)
    {
        await context.UserGroupsMessages.Where(x => x.FromUserId == fromUserId)
        .ExecuteUpdateAsync(s => s
        .SetProperty(e => e.FromUserName, e => fromUserName)
        .SetProperty(e => e.FromUserAvatar, e => fromUserAvatar)
        );
    }
    // 新增群聊信息
    public async Task<long> CreateGroupsMessageAsync(CreateUserGroupsMessageEntity e)
    {
        UserGroupsMessage userGroupsMessage = new(e.UserGroupsId, e.FromUserId, e.FromUserName, e.FromUserAvatar, e.PostMessagese);
        try
        {
            await context.UserGroupsMessages.AddAsync(userGroupsMessage);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return 0;
        }
        await userHubService.SendDataByUserIdAsync(e.ReceiveUserIds, "CreateGroupsMessage", userGroupsMessage);
        return userGroupsMessage.Id;
    }

    // 更新群聊信息用户的读取状态为：已读
    public async Task ReadUserGroupsMessageAsync(long userGroupsId, long toUserId, IEnumerable<long> readMessageIds)
    {
        // 删除未读，就代表已读
        await context.UserGroupsMessageUserUnreads.Where(x => x.UserGroupsId == userGroupsId &&
            x.ToUserId == toUserId && readMessageIds.Contains(x.UserGroupsMessageId)).ExecuteDeleteAsync();
    }
    // 添加群聊信息用户的读取状态为：未读
    public async Task AddUnReadUserGroupsMessageAsync(long userGroupsId, long toUserId, IEnumerable<long> unReadMessageIds)
    {
        List<UserGroupsMessageUserUnread> list = new List<UserGroupsMessageUserUnread>();
        foreach (var unReadMessageId in unReadMessageIds)
        {
            list.Add(new(userGroupsId, toUserId, unReadMessageId));
        }
        await context.BulkInsertAsync(list);
        await context.SaveChangesAsync();
    }
    // 更新群聊信息为删除状态
    public async Task DeleteUserGroupsMessageAsync(long userGroupsId, long toUserId, long deleteMessageId)
    {
        // 添加该新消息的数据，代表该用户删除了该数据
        var data = new UserGroupsMessageUserDeleted(userGroupsId, toUserId, deleteMessageId);
        await context.UserGroupsMessageUserDeleteds.AddAsync(data);
        await context.SaveChangesAsync();
    }

    // 更新群聊用户关联表的删除时间
    public async Task DeleteUserGroupsToUserAsync(long userId, long userGroupsId)
    {
        var data = await context.UserGroupsToUsers
            .FirstAsync(x => x.UserId == userId && x.UserGroupsId == userGroupsId);
        data.UpdateDeletionTime();
        await context.SaveChangesAsync();
    }

}
