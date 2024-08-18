using UserChatService.Domain.Entities.UserChat;
using UserChatService.Domain.Model.Request;
using UserChatService.Domain.Model.Response;

namespace UserChatService.Domain.IService
{
    public interface IUserDialogService
    {
        Task<long> CreateDialogMessageAsync(CreateUserDialogMessageEntity e);
        Task<long> CreateUserDialogAsync(CreateUserDialogEntity e);
        Task DeleteUserDialogToUserAsync(long userId, long userDialogId);
        Task DeleteUserGroupsToUserAsync(long userId, long userGroupsId);
        Task<IEnumerable<UserDialogToUserDTO>> GetDialogByUserIdAsync(long userId);
        Task<(IEnumerable<UserDialogMessage> list, bool over)> GetDialogMessageByDialogIdAsync(long userId, long dialogId, int pageSize, long beginId = 0);
        Task UpdateDialogToUserAsync(long toUserId, string toUserName, string toUserAvatar);
        Task DeleteUserDialogMessageAsync(long userId, long deleteMessageId);
        Task ReadUserDialogMessageAsync(long userDialogId, long fromUserId, long toUserId, IEnumerable<long> readMessageIds);
    }
}