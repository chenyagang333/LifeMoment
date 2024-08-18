using AutoMapper;
using Chen.ASPNETCore;
using Chen.Commons;
using Chen.Commons.ApiResult;
using Chen.DomainCommons.ConfigOptions;
using IdentityService.Domain.DTO;
using IdentityService.Domain.Entities;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.IService;
using IdentityService.Domain.Notifications;
using IdentityService.Infrastructure;
using IdentityService.WebAPI.RequestObject.User;
using IdentityService.WebAPI.ResponseObject.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Transactions;

namespace IdentityService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRespository userRespository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRelevant userRelevant;
        private readonly UserDbContext userDbContext;
        private readonly IMapper mapper;
        private readonly IMediator _mediator;
        private readonly IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions;

        public UserController(
            IUserRespository userRespository,
            IHttpContextAccessor httpContextAccessor,
            IUserRelevant userRelevant,
            UserDbContext userDbContext,
            IMapper mapper,
            IMediator mediator,
            IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions
            )
        {
            this.userRespository = userRespository;
            this.httpContextAccessor = httpContextAccessor;
            this.userRelevant = userRelevant;
            this.userDbContext = userDbContext;
            this.mapper = mapper;
            _mediator = mediator;
            this.fileServerOptions = fileServerOptions;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> GetUserDataByToken()
        {
            var addressIp = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            string? userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdStr != null && long.TryParse(userIdStr, out long userId))
            {
                User? user = await userRespository.FindByUserIdAsync(userId);
                if (user != null)
                {
                    var userDTO = mapper.Map<User, UserDTO>(user);
                    userDTO.UpdateUserAvatar(fileServerOptions.Value.FileBaseUrl);
                    return ApiResult.Succeeded(userDTO);
                }
            }
            return ApiResult.Failed("未找到用户信息");
        }

        /// <summary>
        /// 根据用户Id获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult>> GetUserDataById(long userId)
        {
            User? user = await userRespository.FindByUserIdAsync(userId);
            if (user == null) return ApiResult.Failed("未找到用户信息");
            var userDTO = mapper.Map<User, UserDTO>(user);
            userDTO.UpdateUserAvatar(fileServerOptions.Value.FileBaseUrl);
            return ApiResult.Succeeded(userDTO);
        }
        ///// <summary>
        ///// 获取用户的签到状态
        ///// </summary>
        ///// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<bool>> GetSignInState(long userId)
        {
            var state = await userRelevant.GetSignInStateAsync(userId);
            return state;

        }
        ///// <summary>
        ///// 获取用户本月的签到次数
        ///// </summary>
        ///// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<int>> GetCountOfSignInForMonth(long userId)
        {
            var count = await userRelevant.GetCountOfSignInForMonthAsync(userId);
            return count;

        }

        /// <summary>
        /// 获取关注状态—— 0：谁也没关注对方，1：代表我关注TA，2：代表TA关注我，3：代表互相关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> GetAttentionState(long userId, long toUserId)
        {
            var state = await userRelevant.GetAttentionStateAsync(userId, toUserId);
            return ApiResult.Succeeded(state);
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(typeof(UserDbContext))]
        [HttpPost]
        public async Task<ActionResult<ApiResult>> EditUserData(EditUserData editUserData)
        {
            var userId = HttpHelper.TryGetUserId(HttpContext);
            if (userId > 0)
            {
                User? user = await userRespository.FindByUserIdAsync(userId);
                if (user == null) return ApiResult.Failed("未找到用户信息");
                user.UpdateUserName(editUserData.UserName);
                user.ChangeUserAvatar(editUserData.UserAvatar);
                user.UpdateDescription(editUserData.Description);
                await userDbContext.SaveChangesAsync();
                // 领域实体保存成功后，发布领域事件
                await _mediator.DispatchDomainEventsAsync(userDbContext);
            }
            return ApiResult.Succeeded("修改成功！");
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(typeof(UserDbContext))]
        [HttpGet]
        public async Task<ActionResult<ApiResult>> SignIn(long userId)
        {
            var res = await userRelevant.SignInAsync(userId);
            if (res.Succeeded)
            {
                return ApiResult.Succeeded("签到成功！");
            }
            return ApiResult.Failed(res.Description);
        }


        /// <summary>
        /// 点关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [UnitOfWork(typeof(UserDbContext))]
        [HttpGet]
        public async Task<ActionResult<ApiResult>> UserAddAttention(long userId, long toUserId)
        {
            await userRelevant.UserAddAttentionAsync(userId, toUserId);
            using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
            await userDbContext.SaveChangesAsync();
            transactionScope.Complete();
            // 领域实体保存成功后，发布领域事件
            await _mediator.DispatchDomainEventsAsync(userDbContext);
            return ApiResult.Succeess;
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [UnitOfWork(typeof(UserDbContext))]
        [HttpGet]

        public async Task<ActionResult<ApiResult>> UserCancelAttention(long userId, long toUserId)
        {
            await userRelevant.UserCancelAttentionAsync(userId, toUserId);
            using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
            await userDbContext.SaveChangesAsync();
            transactionScope.Complete();
            // 领域实体保存成功后，发布领域事件
            await _mediator.DispatchDomainEventsAsync(userDbContext);
            return ApiResult.Succeess;
        }




	
	}
}
