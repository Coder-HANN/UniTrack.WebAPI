using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, ServiceResponse<List<GetAllUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        
        public GetAllUserQueryHandler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
        }
        public async Task<ServiceResponse<List<GetAllUserQueryResponseDTO>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<List<GetAllUserQueryResponseDTO>> {
                    
                        IsSuccess = false,
                        Data = null,
                        Message = "Unauthorized"
                    
                };
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.Club || role == Role.User)
            {
                 return new ServiceResponse<List<GetAllUserQueryResponseDTO>>
                 { 
                    
                         IsSuccess = false,
                         Data = null,
                         Message = "Yetkisiz kullanıcı"
                     
                 };
            }

            var users = await userRepository.GetAllAsync();
            if (users == null || users.Count == 0)
            {
                return new ServiceResponse<List<GetAllUserQueryResponseDTO>> {
                        IsSuccess = false,
                        Data = null,
                        Message = "No users found"
                };
            }

            var responses = users.Select(u => new GetAllUserQueryResponseDTO
            {
               
                    UserId = u.Id,
                    Name = u.UserDetail.Name,
                    Surname = u.UserDetail.Surname,
                    Email = u.Email,
                    UniversityId = u.UserDetail.UniverstiyId,
                    DepartmentId = u.UserDetail.DepartmentId,
                
            }).ToList();

            return new ServiceResponse<List<GetAllUserQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = responses,
                Message = "Toplam kullanıcı sayısı: " + responses.Count
            };
        }
    }
}
