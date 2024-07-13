using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.IService
{
    public interface IUserService
    {
        Task<string> GetRandomUserNameAsync();
        Task<string> GetRandomAvatarAsync();
    }
}
