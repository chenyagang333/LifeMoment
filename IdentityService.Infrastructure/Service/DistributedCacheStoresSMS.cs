using IdentityService.Domain.IService;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class DistributedCacheStoresSMS : IDistributedCacheStoresSMS
    {
        private readonly ISmsSender smsSender;
        private readonly IDistributedCache distributedCache;
        public DistributedCacheStoresSMS(ISmsSender smsSender, IDistributedCache distributedCache)
        {
            this.smsSender = smsSender;
            this.distributedCache = distributedCache;
        }
        public Task SetSMSAsync(string key, int expiredMinutes, string phoneNumber, string info)
        {
            return Task.CompletedTask;
        }
    }
}
