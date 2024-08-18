using Chen.Commons;
using Chen.Commons.ApiResult;
using Chen.Commons.ApiResult.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserChatService.Domain.IService;
using UserChatService.Domain.Model.Request;

namespace UserChatService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class UserGroupsController : ControllerBase
    {
        private readonly IUserGroupsService userGroupsService;

        public UserGroupsController(IUserGroupsService userGroupsService)
        {
            this.userGroupsService = userGroupsService;
        }
        // 创建群聊
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateUserGroups(CreateUserGroupsEntity e)
        {
            var id = await userGroupsService.CreateUserGroupsAsync(e);
            return ApiResult.Succeeded(id);
        }

        // 创建群聊消息
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateGroupsMessage(CreateUserGroupsMessageEntity e)
        {
            var id = await userGroupsService.CreateGroupsMessageAsync(e);
            return ApiResult.Succeeded(id);
        }

        // 删除群聊
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserGroupsToUser(long userId, long userGroupsId)
        {
            await userGroupsService.DeleteUserGroupsToUserAsync(userId, userGroupsId);
            return ApiResult.Succeess;
        }

        // 删除群聊消息
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserGroupsMessage(long userGroupsId, long toUserId, long deleteMessageId)
        {
            await userGroupsService.DeleteUserGroupsMessageAsync(userGroupsId, toUserId, deleteMessageId);
            return ApiResult.Succeess;
        }

        // 修改群聊消息读取状态：读取
        [HttpPut]
        public async Task<ActionResult<ApiResult>> ReadUserGroupsMessage(ReadUserGroupsMessageRequest e)
        {
            await userGroupsService.ReadUserGroupsMessageAsync(e.userGroupsId, e.toUserId, e.readMessageIds);
            return ApiResult.Succeess;
        }
    }
}
