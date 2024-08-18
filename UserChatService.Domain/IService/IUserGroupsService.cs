using UserChatService.Domain.Model.Request;
using UserChatService.Domain.Model.Response;

namespace UserChatService.Domain.IService
{
    public interface IUserGroupsService
    {
        Task AddUnReadUserGroupsMessageAsync(long userGroupsId, long toUserId, IEnumerable<long> unReadMessageIds);
        Task<long> CreateGroupsMessageAsync(CreateUserGroupsMessageEntity e);
        Task<long> CreateUserGroupsAsync(CreateUserGroupsEntity e);
        Task DeleteUserGroupsMessageAsync(long userGroupsId, long toUserId, long deleteMessageId);
        Task<IEnumerable<UserGroupsToUserDTO>> GetGroupsByUserIdAsync(long userId);
        Task<(IEnumerable<UserGroupsMessageDTO> list, bool over)> GetUserGroupsMessageByUserGroupsIdAsync(long userId, long userGroupsId, int pageSize, long beginId = 0);
        Task ReadUserGroupsMessageAsync(long userGroupsId, long toUserId, IEnumerable<long> readMessageIds);
        Task UpdateUserGroupsMessageAsync(long fromUserId, string fromUserName, string fromUserAvatar);
        Task UpdateUserGroupsToUserAsync(long userGroupsId, string name, string icon);
        Task DeleteUserGroupsToUserAsync(long userId, long userGroupsId);
    }
}