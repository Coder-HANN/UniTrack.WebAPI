using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Feature.Event.Command.EventCheckInCommandHandler;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Domain.Entities;
using UniTrack.Application.Common.Constants;

public class EventCheckInCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IParticipantSheetRepository> _sheetRepositoryMock = new();
    private readonly Mock<ICurrentUserServices> _currentUserServicesMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();

    private readonly EventCheckInCommandHandler _handler;

    public EventCheckInCommandHandlerTests()
    {
        _handler = new EventCheckInCommandHandler(
            _eventRepositoryMock.Object,
            _userRepositoryMock.Object,
            _sheetRepositoryMock.Object,
            _currentUserServicesMock.Object,
            _eventUserRepositoryMock.Object,
            _localizationMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCheckInSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserServicesMock
            .Setup(x => x.CurrentUser())
            .Returns(userId);

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync(new Event
            {
                Id = eventId,
                SheetsId = "sheet-123"
            });

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User
            {
                Id = userId,
                Email = "test@unitrack.com"
            });

        _eventUserRepositoryMock
            .Setup(x => x.GetEventUserCheckInAsync(userId, eventId))
            .ReturnsAsync((EventUser)null);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.CheckInSuccess))
            .ReturnsAsync("Check-in successful");

        var command = new EventCheckInCommand
        {
            EventCheckInId = eventId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Check-in successful");

        _sheetRepositoryMock.Verify(x =>
            x.MarkUserAsCheckedInAsync("sheet-123", "test@unitrack.com"),
            Times.Once);

        _eventUserRepositoryMock.Verify(x =>
            x.UpdateAsync(It.Is<EventUser>(eu =>
                eu.EventId == eventId &&
                eu.UserId == userId &&
                eu.IsCheckedIn &&
                eu.IsJoined)),
            Times.Once);
    }
}
