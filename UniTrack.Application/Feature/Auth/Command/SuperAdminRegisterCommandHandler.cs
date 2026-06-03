using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class SuperAdminRegisterCommandHandler : IRequestHandler<SuperAdminRegisterCommand, ServiceResponse<SuperAdminRegisterResponseDTO>>
    {
        Task<ServiceResponse<SuperAdminRegisterResponseDTO>> IRequestHandler<SuperAdminRegisterCommand, ServiceResponse<SuperAdminRegisterResponseDTO>>.Handle(SuperAdminRegisterCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
