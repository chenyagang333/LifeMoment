using Chen.Commons.FunResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.IService
{
    public interface IEmailSender
    {
        public Task<FunResult> SendAsync(string toEmail, string subject, string body);

    }
}
