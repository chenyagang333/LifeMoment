using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.IService
{
    public interface IDistributedCacheStoresSMS
    {
        Task SetSMSAsync(string key,int expiredMinutes,string phoneNumber,string info);
    }
}
