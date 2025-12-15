using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class CompanyRegisterCommandHandler : IRequestHandler<CompanyRegisterCommand, ServiceResponse<CompanyRegisterResponseDTO>>
    {
        Task<ServiceResponse<CompanyRegisterResponseDTO>> IRequestHandler<CompanyRegisterCommand, ServiceResponse<CompanyRegisterResponseDTO>>.Handle(CompanyRegisterCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
