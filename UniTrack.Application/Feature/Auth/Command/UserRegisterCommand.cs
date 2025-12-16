using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class UserRegisterCommand : IRequest<ServiceResponse<UserRegisterResponseDTO>>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int DepartmentId { get; set; }
        public Guid UniversityId { get; set; }
        public int CityId { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public long PhoneNumber { get; set; }
        public DateTime Graduaiton_Date { get; set; }

    }
}
