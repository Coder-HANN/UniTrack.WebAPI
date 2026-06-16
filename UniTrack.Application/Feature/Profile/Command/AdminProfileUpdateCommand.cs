using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class AdminProfileUpdateCommand : IRequest<ServiceResponse<AdminProfileUpdateResponseDTO>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? NowPassword { get; set; }
        public string? Name { get; set; }
        public Guid? UniverstiyId { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool? IsNotified { get; set; }
    }
}
