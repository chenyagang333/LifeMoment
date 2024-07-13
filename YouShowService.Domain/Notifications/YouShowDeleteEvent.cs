using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.Notifications
{
    public record YouShowDeleteEvent(long YoushowId) : INotification;
}
