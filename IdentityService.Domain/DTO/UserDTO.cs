using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.DTO
{
    public record UserDTO
    {
        public string? Ip { get; set; }
        public long Id { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string userAvatarRelativeUrl { get; set; }
        public int UserAccount { get; set; }
        public int AttentionCount { get; set; }
        public int FansCount { get; set; }
        public long GetLikeCount { get; set; }
        public int LikeCount { get; set; }
        public int StarCount { get; set; }
        public int ContentCount { get; set; }
        public string Description { get; set; }

        public UserDTO UpdateUserAvatar(string baseUrl)
        {
            if (!string.IsNullOrEmpty(UserAvatar))
            {
                userAvatarRelativeUrl = UserAvatar;
                UserAvatar = baseUrl + UserAvatar;
            }
            return this;
        }
    };
}
