using MediatR;
using UniTrack.Application.Common;

public class ResetPasswordCommand : IRequest<ServiceResponse<string>>
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}