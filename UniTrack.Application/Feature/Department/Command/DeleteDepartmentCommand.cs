using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Department.Command
{
    public class DeleteDepartmentCommand : IRequest<ServiceResponse<string>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
