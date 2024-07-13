using IdentityService.Domain.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Service
{
    public class MockSmsSender : ISmsSender
    {
        private readonly ILogger<MockSmsSender> logger;

        public MockSmsSender(ILogger<MockSmsSender> logger)
        {
            this.logger = logger;
        }   
        public Task SendAsync(string phoneNum, params string[] args)
        {

            logger.LogInformation("Send Sms to {0},args:{1}",phoneNum,string.Join(", ",args));
            return Task.CompletedTask;
        }
    }
}
