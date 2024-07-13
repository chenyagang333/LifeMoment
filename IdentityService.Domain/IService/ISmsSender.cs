using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.IService
{
    public interface ISmsSender
    {
        public Task SendAsync(string phoneNum, params string[] args);

    }
}
