using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Respository;
using IdentityService.Infrastructure;
using IdentityService.WebAPI.RequestObject.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IdentityService.WebAPI.RequestObject.Login;
using IdentityService.Domain.IRespository;
using IdentityService.Domain;
using IdentityService.Domain.IService;
using Microsoft.Extensions.Caching.Distributed;
using Chen.Commons.ApiResult;

namespace IdentityService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private string CodeLoginInstanceName => "CodeLogin_";

        private readonly IUserRespository userRespository;
        private readonly UserDomainService userDomainService;
        private readonly ISmsSender smsSender;
        private readonly IEmailSender emailSender;
        private readonly IDistributedCache distributedCache;

        public LoginController(IUserRespository userRespository, UserDomainService userDomainService, ISmsSender smsSender, IDistributedCache distributedCache, IEmailSender emailSender)
        {
            this.userRespository = userRespository;
            this.userDomainService = userDomainService;
            this.smsSender = smsSender;
            this.distributedCache = distributedCache;
            this.emailSender = emailSender;
        }
        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResult>> Login(LoginDTO dto)
        {
            var (succeed, token) = await userDomainService.LoginAsync(dto.Account, dto.Password);
            if (succeed)
            {
                return ApiResult.Succeeded(token!);
            }
            return ApiResult.Failed("用户信息或密码输入有误！");
        }

        /// <summary>
        /// 手机验证码登录，用户不存在则创建用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResult>> LoginByPhoneSMS(LoginBySMSDTO dto)
        {
            string? code = await distributedCache.GetStringAsync($"{CodeLoginInstanceName}{dto.PhoneNumber}");
            if (code == null)
            {
                return ApiResult.Failed("请先发送验证码！");
            }
            if (code != dto.Code)
            {
                return ApiResult.Failed("验证码有误！");
            }
            var (res,token) = await userDomainService.LoginByPhoneSMSAsync(dto.PhoneNumber);
            if (res.Succeeded)
            {
                return ApiResult.Succeeded(token);
            }
            return ApiResult.Failed("用户信息或密码输入有误！");
        }

        /// <summary>
        /// 邮箱验证码登录，用户不存在则创建用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResult>> LoginByMail(LoginByMailDTO dto)
        {
            string? code = await distributedCache.GetStringAsync($"{CodeLoginInstanceName}{dto.Mail}");
            if (code == null)
            {
                return ApiResult.Failed("请先发送验证码！");
            }
            if (code != dto.Code)
            {
                return ApiResult.Failed("验证码有误！");
            }
            var (res, token) = await userDomainService.LoginByMailAsync(dto.Mail);
            if (res.Succeeded)
            {
                return ApiResult.Succeeded(token);
            }
            return ApiResult.Failed("用户信息或密码输入有误！");
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult>> SendCodeByLogin(string number)
        {
            int code = Random.Shared.Next(100000, 999999);
            int expiredMinutes = 5;
            string info = $"""
                【WeStrength】验证码：{code}。
                 此验证码只用于WeStrength验证码登陆，{expiredMinutes}分钟内有效。
                """;
            var res = await emailSender.SendAsync(number, "WeStrength登录服务", info);
            //await smsSender.SendAsync(phoneNumber, info);
            if (res.Succeeded)
            {
                await distributedCache.SetStringAsync(
                    key: $"{CodeLoginInstanceName}{number}",
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
