using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
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
            IsDeleted = false,
            IsActived = true
        };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club); // Club rolü yeterli

        _eventRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Event, bool>>>()))
            .ReturnsAsync(existingEvent);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.EventDeletedSuccess, It.IsAny<object[]>()))
            .ReturnsAsync("Etkinlik silindi.");

        var command = new DeleteEventCommand { EventId = eventId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Etkinlik silindi.");
        existingEvent.IsDeleted.Should().BeTrue();
        existingEvent.IsActived.Should().BeFalse();

        _eventRepositoryMock.Verify(x => x.UpdateAsync(existingEvent), Times.Once);

        // Notification servisine giden 3 parametreyi doğrula
        _notificationServiceMock.Verify(
            x => x.ClubIsDeleteEventAsync(
                clubId,
                eventId,
                NotificationType.EventDeleted.ToString()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_ClubId_IsNull()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _localizationMock.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync("Yetkisiz.");

        // Act
        var result = await _handler.Handle(new DeleteEventCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Yetkisiz.");
        _eventRepositoryMock.Verify(x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Event, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Event_Not_Exists()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

        _eventRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Event, bool>>>()))
            .ReturnsAsync((UniTrack.Domain.Entities.Event)null);

        _localizationMock.Setup(x => x.Get(ValidationKeys.EventNotFound)).ReturnsAsync("Etkinlik bulunamadı.");

        // Act
        var result = await _handler.Handle(new DeleteEventCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Etkinlik bulunamadı.");
    }
}