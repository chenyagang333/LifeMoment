using Chen.Commons.ApiResult;
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
    public class UserDialogController : ControllerBase
    {
        private readonly IUserDialogService userDialogService;

        public UserDialogController(IUserDialogService userDialogService)
        {
            this.userDialogService = userDialogService;
        }

        // 创建私聊
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateUserDialog(CreateUserDialogEntity e)
        {
            var id = await userDialogService.CreateUserDialogAsync(e);
            return ApiResult.Succeeded(id);
        }
        // 创建私聊消息
        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateDialogMessage(CreateUserDialogMessageEntity e)
        {
            var id = await userDialogService.CreateDialogMessageAsync(e);
            return ApiResult.Succeeded(id);
        }
        // 删除私聊
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserDialogToUser(long userId, long userDialogId)
        {
            await userDialogService.DeleteUserDialogToUserAsync(userId, userDialogId);
            return ApiResult.Succeess;
        }
        // 删除私聊消息
        [HttpDelete]
        public async Task<ActionResult<ApiResult>> DeleteUserDialogMessage(long userId, long deleteMessageId)
        {
            await userDialogService.DeleteUserDialogMessageAsync(userId, deleteMessageId);
            return ApiResult.Succeess;
        }
        // 修改私聊消息读取状态：读取
        [HttpPut]
        public async Task<ActionResult<ApiResult>> ReadUserDialogMessage(ReadUserDialogMessageRequest e)
        {
            await userDialogService.ReadUserDialogMessageAsync(e.userDialogId, e.fromUserId, e.toUserId, e.readMessageIds);
            return ApiResult.Succeess;
        }
    }
}
