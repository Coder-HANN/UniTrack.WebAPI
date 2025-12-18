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

public class CloseNotificationForClubCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
    private readonly Mock<IUserClubRepository> _userClubRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly CloseNotificationForClubCommandHandler _handler;

    public CloseNotificationForClubCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserServices>();
        _userClubRepositoryMock = new Mock<IUserClubRepository>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _handler = new CloseNotificationForClubCommandHandler(
            _currentUserServiceMock.Object,
            _userClubRepositoryMock.Object,
            _localizationServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_NotificationAlreadyClosed_ShouldReturnError()
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
            .Setup(x => x.Get(ValidationKeys.NotificationAlreadyClosed))
            .ReturnsAsync("Notification already closed");

        var command = new CloseNotificationForClubCommand { ClubId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Notification already closed", result.Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCloseNotification()
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
            .Setup(x => x.Get(ValidationKeys.NotificationClosedSuccess))
            .ReturnsAsync("Notification closed");

        var command = new CloseNotificationForClubCommand { ClubId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(userClub.IsNotification);
        Assert.Equal("Notification closed", result.Message);

        _userClubRepositoryMock.Verify(x => x.UpdateAsync(userClub), Times.Once);
    }
}
