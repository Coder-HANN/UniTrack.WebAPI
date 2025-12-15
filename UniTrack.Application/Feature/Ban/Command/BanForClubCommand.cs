using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanForClubCommand : IRequest<ServiceResponse<string>>
    {
        public Guid ClubId { get; set; }
        public DateTime LastDate { get; set; }
        public string Description { get; set; }
    }
}
