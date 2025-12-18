using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using UniTrack.Application.Feature.Notification.Command;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;

public class OpenNotificationForClubCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
    private readonly Mock<IUserClubRepository> _userClubRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly OpenNotificationForClubCommandHandler _handler;

    public OpenNotificationForClubCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserServices>();
        _userClubRepositoryMock = new Mock<IUserClubRepository>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _handler = new OpenNotificationForClubCommandHandler(
            _currentUserServiceMock.Object,
            _userClubRepositoryMock.Object,
            _localizationServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_UserNotLoggedIn_ShouldReturnNotAuthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _localizationServiceMock
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not Authorized");

        var command = new OpenNotificationForClubCommand { ClubId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Not Authorized", result.Message);
    }

    [Fact]
    public async Task Handle_UserNotFollowingClub_ShouldReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _userClubRepositoryMock
            .Setup(x => x.GetUserIdInClubAsync(It.IsAny<Guid>(), userId))
            .ReturnsAsync((UserClub)null);

        _localizationServiceMock
            .Setup(x => x.Get(ValidationKeys.UserMustFollowClub))
            .ReturnsAsync("User must follow club");

        var command = new OpenNotificationForClubCommand { ClubId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User must follow club", result.Message);
    }

    [Fact]
    public async Task Handle_NotificationAlreadyOpened_ShouldReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var userClub = new UserClub
        {
            IsNotification = true
        };

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _userClubRepositoryMock
            .Setup(x => x.GetUserIdInClubAsync(It.IsAny<Guid>(), userId))
            .ReturnsAsync(userClub);

        _localizationServiceMock
            .Setup(x => x.Get(ValidationKeys.NotificationAlreadyOpened))
            .ReturnsAsync("Notification already opened");

        var command = new OpenNotificationForClubCommand { ClubId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Notification already opened", result.Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldOpenNotification()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var userClub = new UserClub
        {
            IsNotification = false
        };

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _userClubRepositoryMock
            .Setup(x => x.GetUserIdInClubAsync(It.IsAny<Guid>(), userId))
            .ReturnsAsync(userClub);

        _localizationServiceMock
            .Setup(x => x.Get(ValidationKeys.NotificationOpenedSuccess))
            .ReturnsAsync("Notification opened");

        var command = new OpenNotificationForClubCommand { ClubId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(userClub.IsNotification);
        Assert.Equal("Notification opened", result.Message);

        _userClubRepositoryMock.Verify(x => x.UpdateAsync(userClub), Times.Once);
    }
}
