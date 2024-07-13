using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Notifications
{
    public record UserDataUpdataEvent(long UserId,string Field,object Value):INotification;
}
