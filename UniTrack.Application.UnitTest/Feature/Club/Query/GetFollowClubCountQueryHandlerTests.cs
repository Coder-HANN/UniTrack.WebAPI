using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Query;
using Xunit;

public class GetFollowClubCountQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<IUserClubRepository> _userClubRepository = new();
    private readonly Mock<LocalizationService> _localizationService;

    public GetFollowClubCountQueryHandlerTests()
    {
        // LocalizationService constructor bağımlılık istiyorsa
        _localizationService = new Mock<LocalizationService>(null);
    }

    private GetFollowClubCountQueryHandler CreateHandler()
        => new(
            _currentUserServices.Object,
            _userClubRepository.Object,
            _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Return_Followed_Club_Count_When_User_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(userId);

        _userClubRepository
            .Setup(x => x.GetFollowedClubCountAsync(userId))
            .ReturnsAsync(5);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetFollowClubCountQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(5);
        result.Message.Should().BeNull();

        _userClubRepository.Verify(
            x => x.GetFollowedClubCountAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_NotAuthorized_When_User_Not_Logged_In()
    {
        // Arrange
        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns((Guid?)null);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not authorized");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetFollowClubCountQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0);
        result.Message.Should().Be("Not authorized");

        _userClubRepository.Verify(
            x => x.GetFollowedClubCountAsync(It.IsAny<Guid>()),
            Times.Never);
    }
}
