using IdentityService.Domain.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class SendCloudSmsSender : ISmsSender
    {
        public Task SendAsync(string phoneNum, params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
