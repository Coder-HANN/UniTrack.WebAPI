using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class UserProfileUpdateCommand : IRequest<ServiceResponse<UserProfileUpdateResponseDTO>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public Guid? UniverstiyId { get; set; }
        public int? DepartmentId { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public byte? ProfileImage { get; set; }
        public bool? IsNotified { get; set; }
        public DateTime? Graduaiton_Date { get; set; }
    }
}
