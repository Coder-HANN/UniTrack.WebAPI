using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class MarkNotificationAsReadCommand : IRequest<ServiceResponse<string>>
    {
        public Guid NotificationId { get; set; }
    }

}
