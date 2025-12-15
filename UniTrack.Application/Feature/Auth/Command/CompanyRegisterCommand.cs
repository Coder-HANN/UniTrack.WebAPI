using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class CompanyRegisterCommand : IRequest<ServiceResponse<CompanyRegisterResponseDTO>>
    {
    }
}
