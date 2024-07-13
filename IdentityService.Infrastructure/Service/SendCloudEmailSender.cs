using Chen.Commons;
using Chen.Commons.FunResult;
using Chen.Infrastructure.EmailCore;
using IdentityService.Domain.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class SendCloudEmailSender : IEmailSender
    {
        private readonly ILogger<MockEmailSender> logger;
        private readonly EmailBus emailBus;

        public SendCloudEmailSender(ILogger<MockEmailSender> logger, EmailBus emailBus)
        {
            this.logger = logger;
            this.emailBus = emailBus;
        }

        public async Task<FunResult> SendAsync(string toEmail, string subject, string body)
        {
            if (!RegexHelper.IsValidEmail(toEmail))
            {
                return FunResult.Failed("您输入的邮箱格式有误！");
            }
            try
            {
                var res = await emailBus.SendEmailAsync(toEmail, subject, body);
                logger.LogInformation("Send Email to {0},title:{1}, body:{2}", toEmail, subject, body);
                return FunResult.Success;
            }
            catch (Exception)
            {
                return FunResult.Failed("邮箱发送失败！");
                throw;
            }
        }
    }
}
