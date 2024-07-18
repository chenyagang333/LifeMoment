using AutoMapper;
using Chen.Commons;
using IdentityService.Domain.DTO.UserChat;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.UserChat;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.IService;
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

        public UserChat(UserDbContext context, IMapper mapper, IUserRespository userRespository)
        {
            this.context = context;
            this.mapper = mapper;
            this.userRespository = userRespository;
        }

        #region 获取会话列表

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
                        UnreadCount = x.Count(c => c.ToUserId == userId && !c.Received),
                        LastMessageData = x
                        .OrderByDescending(o => o.Id)
                        .Select(s => new { s.PostMessages, s.CreateTime }).First()
                    }).ToListAsync();
            // 拿到映射后的 对话（私聊）表
            var dialogListDTO = mapper.Map<List<UserDialogToUser>, List<UserDialogToUserDTO>>(dialogList);
            dialogListDTO.ForEach(x =>
            {
                var item = dialogListRelevants.First(f => f.UserDialogId == x.UserDialogId);
                x.UpdateUnreadCount(item.UnreadCount);
                x.UpdateLastMessage(item.LastMessageData.PostMessages).UpdateLastModificationTime(item.LastMessageData.CreateTime);
            });
            return dialogListDTO;

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
                item.UpdateLastMessage(item1.LastData.PostMessages).UpdateLastModificationTime(item1.LastData.CreateTime);
                var item2 = UnreadCounts.First(x => item.UserGroupsId == x.UserGroupsId);
                item.UpdateUnreadCount(item2.UnreadCount);
            });
            return userGroupsToUsersDTO;
        }

        #endregion

        #region 获取会话聊天信息

        // 获取私聊会话信息
        public async Task<IEnumerable<UserDialogMessage>> GetDialogMessageByDialogId(long userId, long dialogId, int pageSize, long beginId = 0)
        {
            // 查询消息表
            var baseQuery = context.UserDialogMessages.Where(x => x.UserDialogId == dialogId);
            if (beginId > 0)
            {
                baseQuery = context.UserDialogMessages.Where(x => x.UserDialogId == dialogId && x.Id < beginId);
            }
            // 获取私聊 聊天记录
            var list = await baseQuery.OrderByDescending(x => x.Id).Paging(pageSize, 1).ToListAsync();
            list.ForEach(x => x.RetractMessageHandler()); // 撤回消息处理
            // 筛选用户删除的数据
            var result = list.Where(x =>
                (x.FromUserId == userId && !x.FromUser_Deleted) ||
                (x.ToUserId == userId && !x.ToUser_Deleted));
            // 防止用户连续删除数据数量大于等于 pageSize 导致查该页数据为0
            if (result.Count() == 0 && beginId > pageSize)
            {
                result = await GetDialogMessageByDialogId(userId, dialogId, pageSize * 2, beginId);
            }
            return result;
        }

        // 获取群聊会话信息
        public async Task<IEnumerable<UserGroupsMessageDTO>> GetUserGroupsMessageByUserGroupsId(long userId, long userGroupsId, int pageSize, long beginId = 0)
        {
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
            // 该用户删除消息的ID
            var userGroupsMessageUserDeleteds = await context.UserGroupsMessageUserDeleteds
                .Where(x => x.UserGroupsId == userGroupsId && x.ToUserId == userId && userGroupsMessagesIds.Contains(x.UserGroupsMessageId)).ToListAsync();
            // 更新每条信息的读取状态
            userGroupsMessagesDTO.ForEach(x =>
            {
                x.RetractMessageHandler(); // 更新撤回信息
                var data = userGroupsMessageUserDeleteds.FirstOrDefault(f => f.UserGroupsMessageId == x.Id);
                if (data != null) { x.IsDeleted(); }
            });
            // 筛除该用户已删除的数据，如果
            var result = userGroupsMessagesDTO.Where(x => !x.Deleted);
            // 防止用户连续删除数据数量大于等于 pageSize 导致查该页数据为0
            if (result.Count() == 0 && beginId > pageSize)
            {
                result = await GetUserGroupsMessageByUserGroupsId(userId, userGroupsId, pageSize, beginId);
            }
            return result;
        }

        #endregion

        #region 更新会话和群聊消息冗余数据
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

        #region 更新私聊或群聊消息的读取状态

        // 更新私聊信息用户的读取状态为：已读
        public async Task ReadUserDialogMessage(long userDialogId, long toUserId, IEnumerable<long> readMessageIds)
        {
            await context.UserDialogMessages.Where(x => x.UserDialogId == userDialogId &&
                                                        x.ToUserId == toUserId && readMessageIds.Contains(x.Id))
            .ExecuteUpdateAsync(s => s
            .SetProperty(e => e.Received, e => true)
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

        #region 更新私聊或群聊消息的删除状态

        // 更新私聊信息为删除状态
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

        #region 创建私聊对话

        public async Task CreateUserDialog(long userId,long toUserId)
        {
            // 添加私聊会话记录
            var data = new UserDialog();
            await context.UserDialogs.AddAsync(data);
            await context.SaveChangesAsync();
        }

        #endregion
    }
}
