using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public record SignIn(long UserId, DateTime SignInDate) : DomainEvents
    {
    }
}
