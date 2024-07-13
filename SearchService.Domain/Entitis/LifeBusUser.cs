using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Domain.Entitis
{
    public record LifeBusUser
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public int UserAccount { get; set; }
        public string UserAccountString { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int AttentionCount { get; set; }
        public int FansCount { get; set; }
        public long GetLikeCount { get; set; }
        public int LikeCount { get; set; }
        public int StarCount { get; set; }
        public int ContentCount { get; set; }
        public string Description { get; set; }

        public LifeBusUser BuildUserAccountString()
        {
            UserAccountString = UserAccount.ToString();
            return this;
        }
    }
}
