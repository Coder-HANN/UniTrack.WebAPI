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

public class GetClubFollowerCountQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly GetClubFollowerCountQueryHandler _handler;

    public GetClubFollowerCountQueryHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _clubRepository = new Mock<IClubRepository>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new GetClubFollowerCountQueryHandler(
            _currentUserServices.Object,
            _clubRepository.Object,
            _localizationService.Object);
    }

    // ----------------------------------------------------
    // ❌ Unauthorized
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_CurrentClubNull_ReturnsUnauthorized()
    {
        // Arrange
        _currentUserServices
            .Setup(x => x.CurrentClub())
            .Returns((Guid?)null);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not Authorized");

        var query = new GetClubFollowerCountQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0);
        result.Message.Should().Be("Not Authorized");
    }

    // ----------------------------------------------------
    // ❌ Follower count = 0
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_FollowerCountZero_ReturnsFail()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentClub())
            .Returns(clubId);

        _clubRepository
            .Setup(x => x.GetClubFollowerCountAsync(clubId))
            .ReturnsAsync(0);

        var query = new GetClubFollowerCountQuery();

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
    public async Task Handle_FollowerCountGreaterThanZero_ReturnsSuccess()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentClub())
            .Returns(clubId);

        _clubRepository
            .Setup(x => x.GetClubFollowerCountAsync(clubId))
            .ReturnsAsync(42);

        var query = new GetClubFollowerCountQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(42);
        result.Message.Should().BeNull();
    }
}
