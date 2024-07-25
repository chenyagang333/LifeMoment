using IdentityService.Domain.DTO.UserChat;
using IdentityService.Domain.Entities.UserChat;
using IdentityService.Domain.ServiceEntities.UserChat;

namespace IdentityService.Domain.IService
{
    public interface IUserChat
    {
        Task AddUnReadUserGroupsMessageAsync(long userGroupsId, long toUserId, IEnumerable<long> unReadMessageIds);
        Task<long> CreateDialogMessageAsync(CreateUserDialogMessageEntity e);
        Task<long> CreateGroupsMessageAsync(CreateUserGroupsMessageEntity e);
        Task<long> CreateUserDialogAsync(CreateUserDialogEntity e);
        Task<long> CreateUserGroupsAsync(CreateUserGroupsEntity e);
        Task DeleteUserDialogMessageAsync(long userId, long deleteMessageId);
        Task DeleteUserDialogToUserAsync(long userId, long userDialogId);
        Task DeleteUserGroupsMessageAsync(long userGroupsId, long toUserId, long deleteMessageId);
        Task DeleteUserGroupsToUserAsync(long userId, long userGroupsId);
        Task<IEnumerable<dynamic>> GetDialogAndGroupsByUserIdAsync(long userId);
        Task<IEnumerable<UserDialogToUserDTO>> GetDialogByUserIdAsync(long userId);
        Task<(IEnumerable<UserDialogMessage> list, bool over)> GetDialogMessageByDialogIdAsync(long userId, long dialogId, int pageSize, long beginId = 0);
        Task<IEnumerable<UserGroupsToUserDTO>> GetGroupsByUserIdAsync(long userId);
        Task<(IEnumerable<UserGroupsMessageDTO> list, bool over)> GetUserGroupsMessageByUserGroupsIdAsync(long userId, long userGroupsId, int pageSize, long beginId = 0);
        Task ReadUserDialogMessageAsync(long userDialogId, long fromUserId, long toUserId, IEnumerable<long> readMessageIds);
        Task ReadUserGroupsMessageAsync(long userGroupsId, long toUserId, IEnumerable<long> readMessageIds);
        Task SendDataByUserIdAsync(IEnumerable<long> userIds, string method, object data);
        Task SendDataByUserIdAsync(long userId, string method, object data);
        Task UpdateDialogToUserAsync(long toUserId, string toUserName, string toUserAvatar);
        Task UpdateUserGroupsMessageAsync(long fromUserId, string fromUserName, string fromUserAvatar);
        Task UpdateUserGroupsToUserAsync(long userGroupsId, string name, string icon);
    }
}