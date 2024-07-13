using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Respository;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Domain.IRespository;
using IdentityService.WebAPI.RequestObject.User;
using IdentityService.Domain;
using IdentityService.Domain.IService;
using Chen.ASPNETCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using IdentityService.Domain.ValueObjects;
using IdentityService.WebAPI.ResponseObject.User;
using Chen.Commons.ApiResult;

namespace IdentityService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private string userRegisterInstanceName => "userRegister_";

        private readonly UserDomainService userDomainService;
        private readonly ISmsSender smsSender;
        private readonly IEmailSender emailSender;
        private readonly IDistributedCache distributedCache;

        public RegisterController(UserDomainService userDomainService, ISmsSender smsSender, IDistributedCache distributedCache, IEmailSender emailSender)
        {
            this.userDomainService = userDomainService;
            this.smsSender = smsSender;
            this.distributedCache = distributedCache;
            this.emailSender = emailSender;
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateUserByMail(CreateUserByMail obj)
        {
            string? code = await distributedCache.GetStringAsync($"{userRegisterInstanceName}{obj.Mail}");
            if (code != obj.Code)
            {
                return ApiResult.Failed("验证码有误！");
            }
            var (result, userAccount) = await userDomainService.CreateByMailAsync(obj.UserName, obj.Mail, obj.Password);
            if (result.Succeeded)
            {
                return ApiResult.Succeeded(userAccount.ToString());
            }
            return ApiResult.Failed(result.Errors.SumErrors());
        }
        /// <summary>
        /// 注册账号获取手机验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> SendCodeByPhoneNumber(string phoneNumber)
        {
            int code = Random.Shared.Next(100000, 999999);
            int expiredMinutes = 5;
            string info = $"""
                【HUGS】验证码：{code}。
                 此验证码只用于HUGS账号注册，{expiredMinutes}分钟内有效。
                """;
            await smsSender.SendAsync(phoneNumber, info); // 发送短息
            // 将验证码存在缓存中
            await distributedCache.SetStringAsync(
                key: $"{userRegisterInstanceName}{phoneNumber}",
                value: code.ToString(),
                options: new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiredMinutes)
                });
            return ApiResult.Succeess;
        }

        /// <summary>
        /// 注册账号获取邮箱验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> SendCodeByMail(string mail)
        {
            int code = Random.Shared.Next(100000, 999999);
            int expiredMinutes = 5;
            string info = $"""
                【WeStrength】验证码：{code}。
                 此验证码只用于WeStrength账号注册，{expiredMinutes}分钟内有效。
                """;
            // 发送邮箱
            var res = await emailSender.SendAsync(mail, "WeStrength用户注册服务", info);
            if (res.Succeeded)
            {
                // 将验证码存在缓存中
                await distributedCache.SetStringAsync(
                    key: $"{userRegisterInstanceName}{mail}",
                    value: code.ToString(),
                    options: new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiredMinutes)
                    });
                return ApiResult.Succeess;
            }
            return ApiResult.Failed(res.Description);
        }
    }
}
