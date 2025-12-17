using Moq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class FollowClubCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IUserClubRepository> _userClubRepository;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly FollowClubCommandHandler _handler;

    public FollowClubCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _userClubRepository = new Mock<IUserClubRepository>();
        _clubRepository = new Mock<IClubRepository>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new FollowClubCommandHandler(
            _currentUserServices.Object,
            _userClubRepository.Object,
            _clubRepository.Object,
            _localizationService.Object);
    }

    // --------------------------------------------------
    // ❌ NOT AUTHORIZED
    // --------------------------------------------------
    [Theory]
    [InlineData(null, Role.User)]
    [InlineData("user", null)]
    [InlineData("user", Role.Club)]
    public async Task Handle_Should_Fail_When_Not_Authorized(string userFlag, Role? role)
    {
        // Arrange
        var command = new FollowClubCommand { ClubId = Guid.NewGuid() };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(userFlag == null ? (Guid?)null : Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(role);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not authorized");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Not authorized", result.Message);

        _userClubRepository.Verify(x => x.AddAsync(It.IsAny<UserClub>()), Times.Never);
        _clubRepository.Verify(x => x.UpdateAsync(It.IsAny<Club>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ ALREADY FOLLOWING
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Already_Following()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clubId = Guid.NewGuid();

        var command = new FollowClubCommand { ClubId = clubId };

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        _userClubRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserClub, bool>>>()))
            .ReturnsAsync(new UserClub());

        _localizationService
            .Setup(x => x.Get(ValidationKeys.AlreadyFollowingClub))
            .ReturnsAsync("Already following club");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Already following club", result.Message);

        _userClubRepository.Verify(x => x.AddAsync(It.IsAny<UserClub>()), Times.Never);
        _clubRepository.Verify(x => x.UpdateAsync(It.IsAny<Club>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ SUCCESS
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Follow_Club_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clubId = Guid.NewGuid();

        var command = new FollowClubCommand { ClubId = clubId };

        var club = new Club
        {
            Id = clubId,
            Follower = 5
        };

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        _userClubRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserClub, bool>>>()))
            .ReturnsAsync((UserClub)null);

        _clubRepository
     .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>()))
     .ReturnsAsync(club);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.FollowClubSuccess))
            .ReturnsAsync("Follow success");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Follow success", result.Message);

        _userClubRepository.Verify(x => x.AddAsync(It.Is<UserClub>(uc =>
            uc.UserId == userId &&
            uc.ClubId == clubId &&
            uc.IsFollowing
        )), Times.Once);

        _clubRepository.Verify(x => x.UpdateAsync(It.Is<Club>(c =>
            c.Follower == 6
        )), Times.Once);
    }
}
