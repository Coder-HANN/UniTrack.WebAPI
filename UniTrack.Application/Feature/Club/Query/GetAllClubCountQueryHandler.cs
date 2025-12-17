using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetAllClubCountQueryHandler : IRequestHandler<GetAllClubCountQuery, ServiceResponse<long>>
    {
        private readonly IClubRepository clubRepository;
        public GetAllClubCountQueryHandler(IClubRepository clubRepository)
        {
            this.clubRepository = clubRepository;
        }
        public async Task<ServiceResponse<long>> Handle(GetAllClubCountQuery request, CancellationToken cancellationToken)
        {
            var count = await clubRepository.GetClubCountAsync();
            if (count == 0 || count == null)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = null
                };
            }

            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = count,
                Message = null
            };

        }
    }
}
