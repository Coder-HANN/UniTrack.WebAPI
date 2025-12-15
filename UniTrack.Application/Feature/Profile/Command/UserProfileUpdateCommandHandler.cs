using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class UserProfileUpdateCommandHandler : IRequestHandler<UserProfileUpdateCommand, ServiceResponse<UserProfileUpdateResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserDetailRepository userDetailRepository;
        private readonly IUserRepository userRepository;
        public UserProfileUpdateCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserDetailRepository userDetailRepository,
            IUserRepository userRepository)
        {
            this.currentUserServices = currentUserServices;
            this.userDetailRepository = userDetailRepository;
            this.userRepository = userRepository;
        }
        public async Task<ServiceResponse<UserProfileUpdateResponseDTO>> Handle(UserProfileUpdateCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<UserProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.Club)
            {
                return new ServiceResponse<UserProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }
            bool isUpdated = false;
            var userDetail = await userDetailRepository.GetAsync(ud => ud.UserId == userId);
            var user = await userRepository.GetAsync(u => u.Id == userId);

            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != userDetail.Name)
            {
                userDetail.Name = request.Name;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(request.Surname) && request.Surname != userDetail.Surname)
            {
                userDetail.Surname = request.Surname;
                isUpdated = true;
            }
            if (request.DepartmentId.HasValue && request.DepartmentId.Value != userDetail.DepartmentId)
            {
                userDetail.DepartmentId = request.DepartmentId.Value;
                isUpdated = true;
            }

            if (request.UniverstiyId.HasValue && request.UniverstiyId.Value != userDetail.UniverstiyId)
            {
                userDetail.UniverstiyId = request.UniverstiyId.Value;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                user.Email = request.Email;
                isUpdated = true;
            }
            if (request.Gender.HasValue && request.Gender.Value != userDetail.Gender)
            {
                userDetail.Gender = request.Gender.Value;
                isUpdated = true;
            }
            if (request.BirthDate.HasValue && request.BirthDate.Value != userDetail.BirthDate)
            {
                userDetail.BirthDate = request.BirthDate.Value;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.ProfileImage.ToString()) && request.ProfileImage != userDetail.ProfileImage)
            {
                userDetail.ProfileImage = request.ProfileImage;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(request.Password) && request.Password != user.Password)
            {
                user.Password = request.Password;
                isUpdated = true;
            }
            if(!bool.Parse(request.IsNotified.ToString()).Equals(userDetail.IsNotified))  // TO DO: Kontrol et frontentte gelen veri ile
            {
                userDetail.IsNotified = bool.Parse(request.IsNotified.ToString());
                isUpdated = true;
            }
            if(request.Graduaiton_Date.HasValue && request.Graduaiton_Date.Value != userDetail.Graduaiton_Date)
            {
                userDetail.Graduaiton_Date = request.Graduaiton_Date.Value;
                isUpdated = true;
            }

            var allMail = await userRepository.GetAllAsync();
            var mailExists = allMail.Any(c => c.Email == request.Email);
            var ue = await userRepository.GetAllAsync();
            var a = ue.Any(u => u.Email == request.Email);

            if (mailExists || a)
            {
                return new ServiceResponse<UserProfileUpdateResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Bu e-posta zaten kullanılıyor."
                };
            }

            if (!isUpdated)
            {
                return new ServiceResponse<UserProfileUpdateResponseDTO>
                {
                    IsSuccess = true,
                    Data = new UserProfileUpdateResponseDTO
                    {
                        Name = userDetail.Name,
                        Surname = userDetail.Surname,
                        DepartmentId = userDetail.DepartmentId,
                        UniverstiyId = userDetail.UniverstiyId,
                        Email = user.Email,
                        BirthDate = userDetail.BirthDate,
                        Gender = userDetail.Gender
                    }
                };
            }
            await userDetailRepository.UpdateAsync(userDetail);
            await userRepository.UpdateAsync(user);
            return new ServiceResponse<UserProfileUpdateResponseDTO>
            {
                Data = null,
                IsSuccess = true,
                Message = "İşlem başarılı"
            };
        }
    }
}
