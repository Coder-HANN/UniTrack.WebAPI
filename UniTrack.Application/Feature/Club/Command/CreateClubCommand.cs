using MediatR;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Command
{
    public class CreateClubCommand : IRequest<ServiceResponse<string>>
    {
        public string Name { get; set; }
        public Guid UniversityId { get; set; }
        public string President { get; set; }
        public string ContectEmail { get; set; }
        public string? Description { get; set; }
        public DateOnly ClubCreatedDate { get; set; }
        public Tag Tag { get; set; }
    }
}
