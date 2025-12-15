using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UpdateEventCommand : IRequest<ServiceResponse<UpdateEventResponseDTO>>
    {
        public Guid Id { get; set; }
        public byte[]? Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOnly Clock { get; set; }
        public Tag Tag { get; set; }
        public long Quota { get; set; }
        public string Location { get; set; }
        public Status Status { get; set; }
    }
}
