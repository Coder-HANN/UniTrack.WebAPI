using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Domain.Enums;
using Xunit;

public class DeleteEventCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILocalizationService> _localizationMock;

    private readonly DeleteEventCommandHandler _handler;

    public DeleteEventCommandHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new DeleteEventCommandHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object,
            _notificationServiceMock.Object,
            _localizationMock.Object);
    }
    [Fact]
    public async Task Handle_Should_Delete_Event_Successfully()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var clubId = Guid.NewGuid();

        var existingEvent = new UniTrack.Domain.Entities.Event
        {
            Id = eventId,
            Title = "Test Event",
            IsDeleted = false
        };

        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);

        _eventRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Event, bool>>>()))
            .ReturnsAsync(existingEvent);

        _localizationMock
            .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<object[]>()))
            .ReturnsAsync("Success");

        var command = new DeleteEventCommand { EventId = eventId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingEvent.IsDeleted.Should().BeTrue();

        _eventRepositoryMock.Verify(
            x => x.UpdateAsync(existingEvent),
            Times.Once);

        _notificationServiceMock.Verify(
            x => x.ClubIsDeleteEventAsync(
                clubId,
                It.IsAny<string>()),
            Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Event_Not_Exists()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);

        _eventRepositoryMock
      .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Event, bool>>>()))
      .ReturnsAsync((UniTrack.Domain.Entities.Event)null);


        _localizationMock
            .Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Event not found");

        var command = new DeleteEventCommand { EventId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<UniTrack.Domain.Entities.Event>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_Should_Return_NotAuthorized_When_User_Role_Is_User()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not authorized");

        var command = new DeleteEventCommand { EventId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventRepositoryMock.Verify(
            x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Event, bool>>>()),
            Times.Never);
    }



}