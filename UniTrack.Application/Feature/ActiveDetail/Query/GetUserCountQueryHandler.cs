using MediatR;
using UniTrack.Application.Abstraction.Repositories;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetUserCountQueryHandler : IRequestHandler<GetUserCountQuery, long>
    {
        private readonly IUserRepository userRepository;
        public GetUserCountQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<long> Handle(GetUserCountQuery request, CancellationToken cancellationToken)
        {
            var totalUserCount = await userRepository.GetUserCountAsync();

            return totalUserCount;

        }
    }
}
