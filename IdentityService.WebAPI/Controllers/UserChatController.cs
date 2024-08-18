using AutoMapper;
using Chen.Commons;
using Chen.Commons.ApiResult;
using Chen.Commons.ApiResult.Generic;
using IdentityService.Domain.DTO;
using IdentityService.Domain.Entities;
using IdentityService.Domain.IService;
using IdentityService.Domain.ServiceEntities.UserChat;
using IdentityService.Infrastructure.Respository;
using IdentityService.WebAPI.RequestObject.UserChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class UserChatController : ControllerBase
    {
        private readonly IUserChat userChat;

        public UserChatController(IUserChat userChat)
        {
            this.userChat = userChat;
        }
        // 创建私聊
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateUserDialog(CreateUserDialogEntity e)
        {
            var id = await userChat.CreateUserDialogAsync(e);
            return ApiResult.Succeeded(id);
        }
        // 创建群聊
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateUserGroups(CreateUserGroupsEntity e)
        {
            var id = await userChat.CreateUserGroupsAsync(e);
            return ApiResult.Succeeded(id);
        }
        // 创建私聊消息
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateDialogMessage(CreateUserDialogMessageEntity e)
        {
            var id = await userChat.CreateDialogMessageAsync(e);
            return ApiResult.Succeeded(id);
        }
        // 创建群聊消息
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateGroupsMessage(CreateUserGroupsMessageEntity e)
        {
            var id = await userChat.CreateGroupsMessageAsync(e);
            return ApiResult.Succeeded(id);
        }
        // 删除私聊
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserDialogToUser(long userId, long userDialogId)
        {
            await userChat.DeleteUserDialogToUserAsync(userId, userDialogId);
            return ApiResult.Succeess;
        }
        // 删除群聊
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserGroupsToUser(long userId, long userGroupsId)
        {
            await userChat.DeleteUserGroupsToUserAsync(userId, userGroupsId);
            return ApiResult.Succeess;
        }
        // 删除私聊消息
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserDialogMessage(long userId, long deleteMessageId)
        {
            await userChat.DeleteUserDialogMessageAsync(userId, deleteMessageId);
            return ApiResult.Succeess;
        }
        // 删除群聊消息
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserGroupsMessage(long userGroupsId, long toUserId, long deleteMessageId)
        {
            await userChat.DeleteUserGroupsMessageAsync(userGroupsId, toUserId, deleteMessageId);
            return ApiResult.Succeess;
        }
        // 修改私聊消息读取状态：读取
        [HttpPut]
        public async Task<ActionResult<ApiResult>> ReadUserDialogMessage(ReadUserDialogMessageRequest e)
        {
            await userChat.ReadUserDialogMessageAsync(e.userDialogId,e.fromUserId, e.toUserId, e.readMessageIds);
            return ApiResult.Succeess;
        }
        // 修改群聊消息读取状态：读取
        [HttpPut]
        public async Task<ActionResult<ApiResult>> ReadUserGroupsMessage(ReadUserGroupsMessageRequest e)
        {
            await userChat.ReadUserGroupsMessageAsync(e.userGroupsId,e.toUserId,e.readMessageIds);
            return ApiResult.Succeess;
        }
        // 查询私聊或群聊对话框
        [HttpGet]
        public async Task<ActionResult<ApiResult<IEnumerable<dynamic>>>> GetDialogAndGroupsByUserId()
        {
            var userId = HttpHelper.TryGetUserId(HttpContext);
            var data = await userChat.GetDialogAndGroupsByUserIdAsync(userId);
            return ApiResult<IEnumerable<dynamic>>.Succeeded(data);
        }
    }
}
