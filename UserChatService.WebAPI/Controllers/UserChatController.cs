using Chen.Commons;
using Chen.Commons.ApiResult;
using Chen.Commons.ApiResult.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserChatService.Domain.IService;
using UserChatService.Domain.Model.Request;

namespace UserChatService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserChatController : ControllerBase
    {
        private readonly IUserChat userChat;

        public UserChatController(IUserChat userChat)
        {
            this.userChat = userChat;
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
