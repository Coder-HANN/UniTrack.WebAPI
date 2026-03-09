using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class UnfollowClubCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IUserClubRepository> _userClubRepository;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly UnfollowClubCommandHandler _handler;

    public UnfollowClubCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _userClubRepository = new Mock<IUserClubRepository>();
        _clubRepository = new Mock<IClubRepository>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new UnfollowClubCommandHandler(
            _currentUserServices.Object,
            _userClubRepository.Object,
            _clubRepository.Object,
            _localizationService.Object);
    }

    // -----------------------------
    // ❌ Unauthorized
    // -----------------------------
    [Fact]
    public async Task Handle_UserNotAuthorized_ReturnsFail()
    {
        // Arrange
        _currentUserServices.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Yetkisiz");

        var command = new UnfollowClubCommand
        {
            ClubId = Guid.NewGuid()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Yetkisiz");
    }

    // -----------------------------
    // ❌ Not Following Club
    // -----------------------------
    [Fact]
    public async Task Handle_UserNotFollowingClub_ReturnsFail()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        _userClubRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserClub, bool>>>()))
            .ReturnsAsync((UserClub)null);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotFollowingClub))
            .ReturnsAsync("Takip etmiyorsunuz");

        var command = new UnfollowClubCommand
        {
            ClubId = Guid.NewGuid()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Takip etmiyorsunuz");
    }

    // -----------------------------
    // ✅ Success
    // -----------------------------
    [Fact]
    public async Task Handle_ValidRequest_UnfollowsClubSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clubId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        var userClub = new UserClub
        {
            UserId = userId,
            ClubId = clubId,
            IsFollowing = true
        };

        var club = new Club
        {
            Id = clubId,
            Follower = 10
        };

        _userClubRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserClub, bool>>>()))
            .ReturnsAsync(userClub);

        _clubRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>()))
            .ReturnsAsync(club);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.UnfollowClubSuccess))
            .ReturnsAsync("Takipten çıkıldı");

        // Act
        var result = await _handler.Handle(
            new UnfollowClubCommand { ClubId = clubId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Takipten çıkıldı");

        _userClubRepository.Verify(x => x.DeleteAsync(userClub), Times.Once);
        _clubRepository.Verify(x => x.UpdateAsync(It.Is<Club>(c => c.Follower == 9)), Times.Once);
    }
}
