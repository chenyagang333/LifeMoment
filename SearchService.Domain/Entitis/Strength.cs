using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Domain.Entitis
{
    public record Strength(
        long Id,
        long UserId,
        string UserName,
        string UserAvatarURL,
        string PublishAddress,
        string Content,
        DateTime CreateTime,
        long ViewCount,
        string? LikeUsers,
        int CommentCount,
        int LikeCount,
        int StarCount,
        int ShareCount
        );
}
