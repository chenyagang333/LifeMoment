using IdentityService.Domain.DTO.UserChat;
using IdentityService.Domain.Entities.UserChat;
using IdentityService.Domain.ServiceEntities.UserChat;

namespace IdentityService.Domain.IService;
public interface IUserChat
{
    Task CreateDialogMessage(long userDialogId, long userId, long toUserId, string message);
    Task CreateGroupsMessage(UserGroupsMessage userGroupsMessage);
    Task CreateUserDialog(CreateUserDialogEntity e);
    Task CreateUserGroups(CreateUserGroupsEntity e);
    Task DeleteUserDialogMessage(long userId, long deleteMessageId);
    Task DeleteUserDialogToUser(long userId, long userDialogId);
    Task DeleteUserGroupsMessage(long userGroupsId, long toUserId, long deleteMessageId);
    Task DeleteUserGroupsToUser(long userId, long userGroupsId);
    Task<IEnumerable<dynamic>> GetDialogAndGroupsByUserId(long userId);
    Task<IEnumerable<UserDialogToUserDTO>> GetDialogByUserId(long userId);
    Task<(IEnumerable<UserDialogMessage> list, bool over)> GetDialogMessageByDialogId(long userId, long dialogId, int pageSize, long beginId = 0);
    Task<IEnumerable<UserGroupsToUserDTO>> GetGroupsByUserId(long userId);
    Task<(IEnumerable<UserGroupsMessageDTO> list, bool over)> GetUserGroupsMessageByUserGroupsId(long userId, long userGroupsId, int pageSize, long beginId = 0);
    Task ReadUserDialogMessage(long userDialogId, long toUserId, IEnumerable<long> readMessageIds);
    Task ReadUserGroupsMessage(long userDialogId, long toUserId, IEnumerable<long> readMessageIds);
    Task UpdateDialogToUser(long toUserId, string toUserName, string toUserAvatar);
    Task UpdateUserGroupsMessage(long fromUserId, string fromUserName, string fromUserAvatar);
    Task UpdateUserGroupsToUser(long userGroupsId, string name, string icon);
    Task SendStatus(long userId, string method, int statusCode);
    Task SendData(long userId, string method, object data);
    Task SendData(IEnumerable<long> userId, string method, object data);
}
