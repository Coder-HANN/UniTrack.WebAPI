using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Department.Command
{
    public class CreateDepartmentCommand : IRequest<ServiceResponse<string>>
    {
        public string Name { get; set; }
    }
}
