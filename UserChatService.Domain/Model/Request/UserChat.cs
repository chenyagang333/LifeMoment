using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserChatService.Domain.Model.Request;
/// <summary>
/// 读取私聊消息请求
/// </summary>
public class ReadUserDialogMessageRequest
{
    public long userDialogId { get; set; }
    public long fromUserId { get; set; }
    public long toUserId { get; set; }
    public IEnumerable<long> readMessageIds { get; set; }
}
/// <summary>
/// 读取群聊消息请求
/// </summary>
public class ReadUserGroupsMessageRequest
{
    public long userGroupsId { get; set; }
    public long toUserId { get; set; }
    public IEnumerable<long> readMessageIds { get; set; }
}
