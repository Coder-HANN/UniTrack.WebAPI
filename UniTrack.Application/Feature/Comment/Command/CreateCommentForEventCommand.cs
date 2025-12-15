using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class CreateCommentForEventCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
        public int Point { get; set; }
        public string? Descripiton  { get; set; }
    }
}
