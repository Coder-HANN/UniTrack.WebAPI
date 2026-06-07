using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.DTOs.Club;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Repositories.Pagenation;
using Xunit;

public class GetFollowClubQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<IClubRepository> _clubRepository = new();
    private readonly Mock<IBaseEntityRepository<UserClub>> _baseEntityRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private GetFollowClubQueryHandler CreateHandler()
        => new(
            _currentUserServices.Object,
            _clubRepository.Object,
            _baseEntityRepository.Object,
            _localizationService.Object);

    // TO DO: Yapılacak

}
