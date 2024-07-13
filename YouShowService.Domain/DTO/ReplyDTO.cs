using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.DTO
{
    public record ReplyDTO : Reply
    {
        public bool LikeActive { get; set; }

        public void CheckLike
            (IEnumerable<long> youShowLikeUsers)
        {
            if (youShowLikeUsers.Contains(this.Id))
            {
                LikeActive = true;
            }
        }
    }

}
