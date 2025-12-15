using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.City.Command
{
    public class CreateCityCommand : IRequest<ServiceResponse<string>>
    {
        public string Name { get; set; }
    }
}
