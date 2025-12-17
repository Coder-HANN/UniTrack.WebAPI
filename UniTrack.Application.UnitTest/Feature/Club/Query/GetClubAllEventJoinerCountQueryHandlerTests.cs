using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Query;
using Xunit;

public class GetClubAllEventJoinerCountQueryHandlerTests
{
    private readonly Mock<IEventUserRepository> _eventUserRepository;
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly GetClubAllEventJoinerCountQueryHandler _handler;

    public GetClubAllEventJoinerCountQueryHandlerTests()
    {
        _eventUserRepository = new Mock<IEventUserRepository>();
        _currentUserServices = new Mock<ICurrentUserServices>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new GetClubAllEventJoinerCountQueryHandler(
            _eventUserRepository.Object,
            _currentUserServices.Object,
            _localizationService.Object);
    }

    // ----------------------------------------------------
    // ❌ Unauthorized (club null or mismatch)
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_ClubIdMismatch_ReturnsUnauthorized()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentClub())
            .Returns(Guid.NewGuid()); // farklı club

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not Authorized");

        var query = new GetClubAllEventJoinerCountQuery
        {
            ClubId = clubId
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0);
        result.Message.Should().Be("Not Authorized");
    }

    // ----------------------------------------------------
    // ❌ Joiner count = 0
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_JoinerCountZero_ReturnsFail()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentClub())
            .Returns(clubId);

        _eventUserRepository
            .Setup(x => x.GetTotalJoinerCountByClubIdAsync(clubId))
            .ReturnsAsync(0);

        var query = new GetClubAllEventJoinerCountQuery
        {
            ClubId = clubId
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0);
        result.Message.Should().BeNull();
    }

    // ----------------------------------------------------
    // ✅ Success
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_JoinerCountGreaterThanZero_ReturnsSuccess()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentClub())
            .Returns(clubId);

        _eventUserRepository
            .Setup(x => x.GetTotalJoinerCountByClubIdAsync(clubId))
            .ReturnsAsync(25);

        var query = new GetClubAllEventJoinerCountQuery
        {
            ClubId = clubId
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(25);
        result.Message.Should().BeNull();
    }
}
