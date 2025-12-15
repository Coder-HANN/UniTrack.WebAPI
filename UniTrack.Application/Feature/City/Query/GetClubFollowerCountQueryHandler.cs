using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.City.Query
{
    public class GetClubFollowerCountQueryHandler : IRequestHandler<GetClubFollowerCountQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubRepository;
        public GetClubFollowerCountQueryHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.clubRepository = clubRepository;
        }
        public async Task<ServiceResponse<long>> Handle(GetClubFollowerCountQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserServices.CurrentClub();
            if (clubId == null)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = "Unauthorized"
                };
            }

            var followerCount = await clubRepository.GetClubFollowerCountAsync(clubId.Value);
            if (followerCount == 0 || followerCount == null)
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
                Data = followerCount,
                Message = null
            };
        }
    }
}
