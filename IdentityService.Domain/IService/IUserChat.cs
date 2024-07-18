using IdentityService.Domain.Entities.UserChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.IService
{
    public interface IUserChat
    {
        Task<IEnumerable<dynamic>> GetDialogAndGroupsByUserId(long userId);
        Task<bool> UpdateDialog(long id);
        Task<IEnumerable<UserDialogMessage>> GetDialogMessageByDialogId(long dialogId, int pageIndex, int pageSize);
        Task<bool> UpdateUserGroups(long userId);
        Task<IEnumerable<UserDialogMessage>> GetUserGroupsMessageByUserGroupsId(long userGroupsId, int pageIndex, int pageSize);
    }
}
