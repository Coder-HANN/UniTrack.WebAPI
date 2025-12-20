using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Common.EventTime;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetAllJoinedEventForUserQueryHandler : IRequestHandler<GetAllJoinedEventForUserQuery, ServiceResponse<IPagingExecutionResult<GetAllJoinedEventForUserQueryResponseDTO>>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventUserRepository eventUserRepository;
        private readonly ILocalizationService localizationService;
        private readonly IBaseEntityRepository<Domain.Entities.EventUser> baseEntityRepository;
        public GetAllJoinedEventForUserQueryHandler(
            ICurrentUserServices currentUserServices,
            IEventUserRepository eventUserRepository,
            ILocalizationService localizationService,
            IBaseEntityRepository<Domain.Entities.EventUser> baseEntityRepository)
        {
            this.currentUserServices = currentUserServices;
            this.eventUserRepository = eventUserRepository;
            this.localizationService = localizationService;
            this.baseEntityRepository = baseEntityRepository;
        }
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllJoinedEventForUserQueryResponseDTO>>> Handle(GetAllJoinedEventForUserQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
            {
                return ServiceResponse<IPagingExecutionResult<GetAllJoinedEventForUserQueryResponseDTO>>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var pagedResult = await eventUserRepository.GetAllJoinedEventForUserAsync(userId);

            if (pagedResult == null || pagedResult.Count == 0)
            {
                return new ServiceResponse<IPagingExecutionResult<GetAllJoinedEventForUserQueryResponseDTO>>
                {
                    IsSuccess = true,
                    Data = await baseEntityRepository.GetPagedResult(
                    Enumerable.Empty<GetAllJoinedEventForUserQueryResponseDTO>(),
                    pageSize: request.PageSize,
                    pageIndex: request.Page,
                    ordering: null,
                    cancellationToken: cancellationToken),
                    Message = await localizationService.Get(ValidationKeys.EventNotFound)
                };
            }

            var now = DateTime.UtcNow;

            var response = pagedResult.Select(e =>
            {
                var eventTime = EventTimeHelper.Calculate(e.Event, now);

                bool? hasAttended = null;

                if (eventTime == Time.Past)
                {
                    hasAttended = e.IsCheckedIn;
                }

                return new GetAllJoinedEventForUserQueryResponseDTO
                {
                    EventImage = e.Event.Image,
                    EventName = e.Event.Title,
                    ShortDescription = e.Event.Description,
                    EventDate = e.Event.StartDate,
                    ClubName = e.Event.Club.Name,
                    UniversityName = e.Event.Club.University.Name,
                    Location = e.Event.Location,
                    EventTag = e.Event.EventTag,
                    Time = eventTime,
                    IsCheckIn = hasAttended,
                    JoinDate = e.CreatedDate
                };
            });

            var result = await baseEntityRepository.GetPagedResult(
               response,
               pageSize: request.PageSize,
               pageIndex: request.Page,
               ordering: q => q.OrderByDescending(x => x.EventDate),
               cancellationToken: cancellationToken
             );

            return new ServiceResponse<IPagingExecutionResult<GetAllJoinedEventForUserQueryResponseDTO>>
            {
                IsSuccess = true,
                Data = result,
                Message = null
            };
        }
    }
}
