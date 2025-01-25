using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.Entities
{
    public record YouShowUserBase(long YouShowId, long UserId) :DomainEvents
    {
        public long Id { get; set; }
    }
}
