using AutoMapper;
using Chen.Commons;
using Chen.Commons.FunResult;
using Chen.JWT;
using IdentityService.Domain.DTO;
using IdentityService.Domain.Entities;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.IService;
using IdentityService.Domain.Notifications;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain;
public class UserDomainService
{
    private readonly IUserRespository userRespository;
    private readonly ITokenService tokenService;
    private readonly IOptions<JWTOptions> optJWT;
    private readonly IUserService userService;
    private readonly IMediator mediator;
    private readonly IMapper mapper;

    public UserDomainService(IUserRespository userRespository, ITokenService tokenService, IOptions<JWTOptions> optJWT, IUserService userService, IMediator mediator, IMapper mapper)
    {
        this.userRespository = userRespository;
        this.tokenService = tokenService;
        this.optJWT = optJWT;
        this.userService = userService;
        this.mediator = mediator;
        this.mapper = mapper;
    }

    /// <summary>
    /// 注册用户
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<(IdentityResult identityResult, long userAccount)> CreateByMailAsync(string userName, string mail, string newPassword)
    {
        if (await userRespository.FindByEmailAsync(mail) != null)
        {
            return (IdentityHelper.ErrorResult("邮箱已存在！"), 0);
        }
        if (await userRespository.FindByUserNameAsync(userName) != null)
        {
            return (IdentityHelper.ErrorResult("用户名已存在！"), 0);
        }
        var (res, user) = await CreateUser(CreateUserType.Email, mail, userName, newPassword);

        return (res, user.UserAccount);
    }

    /// <summary>
    /// HugS用户密码登录
    /// </summary>
    /// <param name="account">HugStrength账号/用户名/手机号/邮箱登</param>
    /// <param name="password">用户密码</param>
    /// <returns>(succeed:是否登录成功，token)</returns>
    public async Task<(bool succeed, string? token)> LoginAsync(string account, string password)
    {
        bool succeeded = false;
        User userData = null;
        void userHandle(User? user)
        {
            if (user != null && user.CheckPassword(password))
            {
                succeeded = true;
                userData = user;
            }
        }
        // 手机号登录 ：判断是否为合法手机号
        if (RegexHelper.IsPhoneNumber(account))
        {
            var user = await userRespository.FindByPhoneNumberAsync(account);
            userHandle(user);
        }
        // 账号登陆 判断是否为合法账号
        if (account.Length == 6 && long.TryParse(account, out long userAccount))
        {
            var user = await userRespository.FindByUserAccountAsync(userAccount);
            userHandle(user);
        }
        // 邮箱登录 :是否为合法邮箱
        if (RegexHelper.IsValidEmail(account))
        {
            var user = await userRespository.FindByEmailAsync(account);
            userHandle(user);
        }
        // 昵称登录 :是否为合法昵称
        if (true)
        {
            var user = await userRespository.FindByUserNameAsync(account);
            userHandle(user);
        }
        return (succeeded, await BuildTokenAsync(userData));
    }

    /// <summary>
    /// HUGS短信登录
    /// </summary>
    /// <param name="phoneNumer"></param>
    /// <returns>(succeed:是否登录成功，token)</returns>
    public async Task<(FunResult, string)> LoginByPhoneSMSAsync(string phoneNumer)
    {
        var user = await userRespository.FindByPhoneNumberAsync(phoneNumer);
        if (user == null)
        {
            var (res, newUser) = await CreateUser(CreateUserType.PhoneNumber, phoneNumer, null, null);
            if (!res.Succeeded)
            {
                return (FunResult.Fail, "");
            }
            user = newUser;
        }
        var token = await BuildTokenAsync(user);
        return (FunResult.Success, token);
    }

    // 邮箱登录
    public async Task<(FunResult, string)> LoginByMailAsync(string mail)
    {
        var user = await userRespository.FindByEmailAsync(mail);
        if (user == null)
        {
            var (res, newUser) = await CreateUser(CreateUserType.Email, mail, null, null);
            if (!res.Succeeded)
            {
                return (FunResult.Fail, "");
            }
            user = newUser;
        }
        var token = await BuildTokenAsync(user);
        return (FunResult.Success, token);
    }



    // 创建新用户
    private async Task<(IdentityResult, User)> CreateUser(CreateUserType type, string number, string? userName, string? password)
    {
        var lastUser = await userRespository.FindLastUserAccountAsync(); // 可能存在并发问题 为用户的账号添加唯一索引
        int userAccount = 0;
        if (lastUser == null)
        {
            userAccount = 100000;
        }
        else
        {
            userAccount = lastUser!.UserAccount + 1;
        }
        if (userName == null)
        {
            userName = await userService.GetRandomUserNameAsync(); // 获取随机用户名
        }
        var userAvatar = await userService.GetRandomAvatarAsync(); // 获取随机用户头像
        User user = new($"{userName}"); // 新用户名为随机昵称和账号组合
        user.ChangeUserAccount(userAccount).
        ChangeUserAvatar(userAvatar);
        if (password != null)
        {
            user.ChangePassword(password);
        }
        if (type == CreateUserType.Email)
        {
            user.ChangeEmail(number);

        }
        else if (type == CreateUserType.PhoneNumber)
        {
            user.ChangePhoneNumber(number);
        }
        IdentityResult res = await userRespository.CreateAsync(user);
        if (res.Succeeded) // 如果用户创建成功，则发布领域事件
        {
            var publishUser = mapper.Map<User, AddUserEvent>(user);
            await mediator.Publish(publishUser); // 通知搜索服务新增用户数据
        }
        return (res, user);
    }

    enum CreateUserType
    {
        Email,
        PhoneNumber
    }



    // 生成jwt Token
    private async Task<string?> BuildTokenAsync(User? user)
    {
        if (user == null) { return default; }

        var roles = await userRespository.GetRolesAsync(user);
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        ];
        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return tokenService.BuildToken(claims, optJWT.Value);
    }


}
