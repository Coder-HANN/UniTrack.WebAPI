using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Command
{
    public class EventIsClubCountCommand : IRequest<ServiceResponse<EventIsClubCountResponseDTO>>
    {
        public Guid ClubId { get; set; }
    }
}
