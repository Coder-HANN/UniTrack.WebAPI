using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class ClubRegisterCommand : IRequest<ServiceResponse<ClubRegisterResponseDTO>>
    {
        public string ClubName { get; set; }
        public string PresidentEmail { get; set; }
        public string ContactEmail { get; set; }
        public string Password { get; set; }
        public string PresidentName { get; set; }
        public Guid UniversityId { get; set; }
        public int CityId { get; set; }    
        public Tag Tag { get; set; }
    }
}
