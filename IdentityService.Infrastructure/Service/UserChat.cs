﻿using AutoMapper;
using Chen.Commons;
using IdentityService.Domain.DTO.UserChat;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.UserChat;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.IService;
using IdentityService.Domain.ServiceEntities.UserChat;
using IdentityService.Infrastructure.Hubs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class UserChat : IUserChat
    {
        private readonly UserDbContext context;
        private readonly IMapper mapper;
        private readonly IUserRespository userRespository;
        private readonly IHubContext<UserChatHub> hubContext;

        public UserChat(UserDbContext context, IMapper mapper, IUserRespository userRespository, IHubContext<UserChatHub> hubContext)
        {
            this.context = context;
            this.mapper = mapper;
            this.userRespository = userRespository;
            this.hubContext = hubContext;
        }


        #region 群聊或私聊会话列表 与用户关联表

        #region Create

        // 创建私聊
        public async Task CreateUserDialog(CreateUserDialogEntity e)
        {
            long dialogId;
            // 检查对话是否存在，
            var userDialogToUser_1 = await context.UserDialogToUsers
                .FirstOrDefaultAsync(x => x.ToUserId == e.toUserId && x.UserId == e.userId);
            var userDialogToUser_2 = await context.UserDialogToUsers
                .FirstOrDefaultAsync(x => x.ToUserId == e.userId && x.UserId == e.toUserId);
            // 有一个为NULL则认为不存在，代表第一次创建对话
            if (userDialogToUser_1 == null || userDialogToUser_2 == null)
            {
                var userDialog = new UserDialog();
                try
                {
                    using var transaction = context.Database.BeginTransaction();
                    // 添加私聊会话记录
                    await context.UserDialogs.AddAsync(userDialog);
                    await context.SaveChangesAsync(); // 保存后拿到Id
                                                      // 添加私聊会话记录用户关联表
                    var userDialogToUser1 = new UserDialogToUser(e.userId, e.toUserId, userDialog.Id, e.toUserName, e.toUserAvatar);
                    var userDialogToUser2 = new UserDialogToUser(e.toUserId, e.userId, userDialog.Id, e.userName, e.userAvatar);
                    List<UserDialogToUser> userDialogToUsers = [userDialogToUser1, userDialogToUser2];
                    await context.BulkInsertAsync(userDialogToUsers);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception)
                {   // 创建对话失败
                    await SendStatus(e.userId, "CreateUserDialogResult", 400);
                    throw;
                }
                // 添加新建对话后的 打招呼消息
                CreateDialogMessage(userDialog.Id, e.userId, e.toUserId, $"👋你好鸭～，我是{e.userName}，聊聊天吧。");
                // 保存更改
                dialogId = userDialog.Id;
            }
            else
            {
                userDialogToUser_1.SoftDelete(false); // 如果删除了聊天，则打开
                userDialogToUser_2.SoftDelete(false); // 
                dialogId = userDialogToUser_1.UserDialogId;
                await context.SaveChangesAsync();
            }
            if (dialogId > 0)
            {
                // 对方的对话框显示发起聊天用户的信息
                var toUserDialog = new UserDialogToUser(e.toUserId, dialogId, e.userId, e.userName, e.userAvatar);
                await SendData(e.toUserId, "CreateUserDialog", toUserDialog);
                // 返回对话框的创建状态
                await SendStatus(e.userId, "CreateUserDialogResult", 200);
            }
        }
        // 创建群聊
        public async Task CreateUserGroups(CreateUserGroupsEntity e)
        {
            var names = e.CreateUserGroupsToUsers.Select(x => new { x.userName });
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
                await SendStatus(e.admainId, "CreateUserGroupsResult", 400);
                throw;
            }
            await SendData(e.CreateUserGroupsToUsers.Select(x => x.userId).Where(x => x != e.admainId), "CreateUserGroups", userGroups);
            await SendStatus(e.admainId, "CreateUserGroupsResult", 200);
            // 新建群聊提示信息
            await CreateGroupsMessage(new(userGroups.Id, 0, "System", "", $"{names} 加入了群聊"));
            // 保存更改
        }

        #endregion

        #region Get

        // 获取会话列表
        public async Task<IEnumerable<dynamic>> GetDialogAndGroupsByUserId(long userId)
        {
            var list1 = await GetDialogByUserId(userId);
            var list2 = await GetGroupsByUserId(userId);
            // 合并
            List<dynamic> dynamics = [.. list1, .. list2];
            return dynamics.OrderByDescending(x => x.TopTime).ThenByDescending(x => x.LastModificationTime);
        }
        // 获取私聊会话列表
        public async Task<IEnumerable<UserDialogToUserDTO>> GetDialogByUserId(long userId)
        {
            // 对话（私聊）表
            var dialogList = await context.UserDialogToUsers.Where(x => x.UserId == userId).ToListAsync();
            // 所有对话表Id
            var userDialogIds = dialogList.Select(x => x.UserDialogId);
            // 根据 UserDialogId 分组，拿到该用户未读消息数、最后一条消息以及时间
            var dialogListRelevants = await context.UserDialogMessages
                    .Where(x => userDialogIds.Contains(x.UserDialogId))
                    .GroupBy(x => x.UserDialogId)
                    .Select(x => new
                    {
                        UserDialogId = x.Key,
                        UnreadCount = x.Count(c => c.ToUserId == userId && !c.MarkRead),
                        LastMessageData = x
                        .OrderByDescending(o => o.Id)
                        .Select(s => new { s.PostMessages, s.CreateTime }).First(),
                    }).ToListAsync();
            // 拿到映射后的 对话（私聊）表
            var dialogListDTO = mapper.Map<List<UserDialogToUser>, List<UserDialogToUserDTO>>(dialogList);
            dialogListDTO.ForEach(x =>
            {
                var item = dialogListRelevants.First(f => f.UserDialogId == x.UserDialogId);
                x.UpdateUnreadCount(item.UnreadCount);
                x.UpdateLastMessage(item.LastMessageData.PostMessages).UpdateLastPostMessageTime(item.LastMessageData.CreateTime);
            });
            // 筛选未删除的对话框
            return dialogListDTO.Where(x => !x.IsDeleted);

        }
        // 回去群聊会话列表
        public async Task<IEnumerable<UserGroupsToUserDTO>> GetGroupsByUserId(long userId)
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
            return userGroupsToUsersDTO.Where(x => !x.IsDeleted);
        }


        #endregion

        #region Update

        // 更新私聊表冗余的用户名、用户图像
        public async Task UpdateDialogToUser(long toUserId, string toUserName, string toUserAvatar)
        {
            await context.UserDialogToUsers.Where(x => x.ToUserId == toUserId)
                        .ExecuteUpdateAsync(s => s
                        .SetProperty(e => e.ToUserName, e => toUserName)
                        .SetProperty(e => e.ToUserAvatar, e => toUserAvatar)
                        );
        }
        // 更新群聊表冗余的群名称、群图像
        public async Task UpdateUserGroupsToUser(long userGroupsId, string name, string icon)
        {
            await context.UserGroupsToUsers.Where(x => x.UserGroupsId == userGroupsId)
            .ExecuteUpdateAsync(s => s
            .SetProperty(e => e.Name, e => name)
            .SetProperty(e => e.Icon, e => icon)
            );
        }

        #endregion

        #region Delete

        // 更新私聊用户关联表的删除时间
        public async Task DeleteUserDialogToUser(long userId, long userDialogId)
        {
            var data = await context.UserDialogToUsers
                .FirstAsync(x => x.UserId == userId && x.UserDialogId == userDialogId);
            data.UpdateDeletionTime();
            await context.SaveChangesAsync();
        }
        // 更新私聊用户关联表的删除时间
        public async Task DeleteUserGroupsToUser(long userId, long userGroupsId)
        {
            var data = await context.UserGroupsToUsers
                .FirstAsync(x => x.UserId == userId && x.UserGroupsId == userGroupsId);
            data.UpdateDeletionTime();
            await context.SaveChangesAsync();
        }

        #endregion

        #endregion

        #region 群聊或私聊的聊天信息

        #region Get 获取群聊或私聊的聊天信息

        // 获取私聊会话信息
        public async Task<(IEnumerable<UserDialogMessage> list, bool over)> GetDialogMessageByDialogId(long userId, long dialogId, int pageSize, long beginId = 0)
        {
            bool over = false; // 提示前端查询是否结束
            // 查询消息表
            var baseQuery = context.UserDialogMessages.Where(x => x.UserDialogId == dialogId);
            if (beginId > 0)
            {
                baseQuery = context.UserDialogMessages.Where(x => x.UserDialogId == dialogId && x.Id < beginId);
            }
            // 获取私聊 聊天记录
            var list = await baseQuery.OrderByDescending(x => x.Id).Paging(pageSize, 1).ToListAsync();
            list.ForEach(x => x.RetractMessageHandler()); // 撤回消息处理
            IEnumerable<UserDialogMessage> result = list;
            // 防止用户连续删除数据数量大于等于 pageSize 导致查该页数据为0
            if (result.Count() > 0 && result.All(x => (x.FromUserId == userId && !x.FromUser_Deleted) ||
                (x.ToUserId == userId && !x.ToUser_Deleted)))
            {
                (result, over) = await GetDialogMessageByDialogId(userId, dialogId, pageSize * 2, beginId);
            }
            // 筛选用户删除的数据
            result = result.Where(x =>
                (x.FromUserId == userId && !x.FromUser_Deleted) ||
                (x.ToUserId == userId && !x.ToUser_Deleted));
            // 查询该对话框用户的删除时间
            var data = await context.UserDialogToUsers.SingleAsync(x => x.UserId == userId && x.UserDialogId == dialogId);
            // 如果查到删除对话以前的消息，则过滤
            if (result.FirstOrDefault(x => x.CreateTime < data.DeletionTime) != null)
            {
                result = result.Where(x => x.CreateTime > data.DeletionTime).ToList(); // 过滤删除对话时间前的消息
                over = true;
            }
            if (result.Count() == 0) { over = true; }
            return (result, over);
        }

        // 获取群聊会话信息
        public async Task<(IEnumerable<UserGroupsMessageDTO> list, bool over)> GetUserGroupsMessageByUserGroupsId(long userId, long userGroupsId, int pageSize, long beginId = 0)
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
            // 该用户删除的消息
            var userGroupsMessageUserDeleteds = await context.UserGroupsMessageUserDeleteds
                .Where(x => x.UserGroupsId == userGroupsId && x.ToUserId == userId && userGroupsMessagesIds.Contains(x.UserGroupsMessageId)).ToListAsync();
            // 该用户删除的消息的ID集合
            var deleteIds = userGroupsMessageUserDeleteds.Select(x => x.UserGroupsMessageId);
            // 更新每条信息的读取状态
            userGroupsMessagesDTO.ForEach(x =>
            {
                x.RetractMessageHandler(); // 更新撤回信息
                if (deleteIds.Contains(x.Id)) { x.IsDeleted(); }
            });
            IEnumerable<UserGroupsMessageDTO> result = userGroupsMessagesDTO;
            // 防止用户连续删除数据数量大于等于 pageSize 导致查该页数据为0
            if (result.Count() > 0 && result.All(x => x.Deleted))
            {
                (result, over) = await GetUserGroupsMessageByUserGroupsId(userId, userGroupsId, pageSize, beginId);
            }
            // 筛除该用户已删除的数据，如果
            result = userGroupsMessagesDTO.Where(x => !x.Deleted);
            // 查询该对话框用户的删除时间
            var data = await context.UserGroupsToUsers.SingleAsync(x => x.UserId == userId && x.UserGroupsId == userGroupsId);
            if (result.FirstOrDefault(x => x.CreateTime < data.DeletionTime) != null)
            {
                result = result.Where(x => x.CreateTime > data.DeletionTime);
                over = true;
            }
            if (result.Count() == 0) { over = true; }
            return (result, over);
        }

        #endregion

        #region Update 更新群聊消息冗余数据

        // 更新群聊 消息 表冗余的用户名、用户图像
        public async Task UpdateUserGroupsMessage(long fromUserId, string fromUserName, string fromUserAvatar)
        {
            await context.UserGroupsMessages.Where(x => x.FromUserId == fromUserId)
            .ExecuteUpdateAsync(s => s
            .SetProperty(e => e.FromUserName, e => fromUserName)
            .SetProperty(e => e.FromUserAvatar, e => fromUserAvatar)
            );
        }

        #endregion

        #region Create 新增私聊或群聊聊天信息

        // 新增私聊信息
        public async Task CreateDialogMessage(long userDialogId, long userId, long toUserId, string message)
        {
            var userDialogMessage = new UserDialogMessage(userDialogId, userId, toUserId)
                .UpdatePostMessages(message);
            try
            {
                await context.UserDialogMessages.AddAsync(userDialogMessage);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                await hubContext.Clients.User(userId.ToString()).SendAsync("CreateDialogMessageResult", new { StatusCode = 400 });
                throw;
            }
            if (userDialogMessage.Id > 0)
            {
                await hubContext.Clients.User(toUserId.ToString()).SendAsync("CreateDialogMessage", new { data = userDialogMessage });
                await hubContext.Clients.User(userId.ToString()).SendAsync("CreateDialogMessageResult", new { StatusCode = 200 });
            }
        }
        // 新增群聊信息
        public async Task CreateGroupsMessage(UserGroupsMessage userGroupsMessage)
        {
            await context.UserGroupsMessages.AddAsync(userGroupsMessage);
            await context.SaveChangesAsync();
        }

        #endregion

        #region Update 更新私聊或群聊消息的读取状态

        // 更新私聊信息用户的读取状态为：已读
        public async Task ReadUserDialogMessage(long userDialogId, long toUserId, IEnumerable<long> readMessageIds)
        {
            await context.UserDialogMessages.Where(x => x.UserDialogId == userDialogId &&
                                                        x.ToUserId == toUserId && readMessageIds.Contains(x.Id))
            .ExecuteUpdateAsync(s => s
            .SetProperty(e => e.MarkRead, e => true)
            );
        }
        // 更新群聊信息用户的读取状态为：已读
        public async Task ReadUserGroupsMessage(long userDialogId, long toUserId, IEnumerable<long> readMessageIds)
        {
            // 删除未读，就代表已读
            await context.UserGroupsMessageUserUnreads.Where(x => x.UserGroupsId == userDialogId &&
                x.ToUserId == toUserId && readMessageIds.Contains(x.UserGroupsMessageId)).ExecuteDeleteAsync();
        }

        #endregion

        #region Delete 更新私聊或群聊消息的删除状态

        // Update 更新私聊信息为删除状态
        public async Task DeleteUserDialogMessage(long userId, long deleteMessageId)
        {
            var data = await context.UserDialogMessages.Where(x => x.Id == deleteMessageId).FirstOrDefaultAsync();
            if (data != null) { data.DeletedHandler(userId); }
            await context.SaveChangesAsync();
        }

        // 更新群聊信息为删除状态
        public async Task DeleteUserGroupsMessage(long userGroupsId, long toUserId, long deleteMessageId)
        {
            // 添加该新消息的数据，代表该用户删除了该数据
            var data = new UserGroupsMessageUserDeleted(userGroupsId, toUserId, deleteMessageId);
            await context.UserGroupsMessageUserDeleteds.AddAsync(data);
            await context.SaveChangesAsync();
        }


        #endregion

        #endregion

        public Task SendStatus(long userId, string method, int statusCode)
        {
            return hubContext.Clients.User(userId.ToString()).SendAsync(method, new { StatusCode = statusCode });
        }

        public Task SendData(long userId, string method, object data)
        {
            return hubContext.Clients.User(userId.ToString()).SendAsync(method, new { data = data });
        }

        public Task SendData(IEnumerable<long> userIds, string method, object data)
        {
            return hubContext.Clients.Users(userIds.Select(x => x.ToString())).SendAsync(method, new { data = data });
        }
    }
}
