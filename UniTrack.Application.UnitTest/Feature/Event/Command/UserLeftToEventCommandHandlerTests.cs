using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class UserLeftToEventCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<IParticipantSheetRepository> _participantSheetRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;

    private readonly UserLeftToEventCommandHandler _handler;

    public UserLeftToEventCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserServices>();
        _eventUserRepositoryMock = new Mock<IEventUserRepository>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _participantSheetRepositoryMock = new Mock<IParticipantSheetRepository>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _handler = new UserLeftToEventCommandHandler(
            _currentUserServiceMock.Object,
            _eventUserRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _participantSheetRepositoryMock.Object,
            _localizationServiceMock.Object);
    }

    // =========================
    // ❌ Kullanıcı giriş yapmamış
    // =========================
    [Fact]
    public async Task Handle_UserNotAuthenticated_ShouldFail()
    {
        _currentUserServiceMock.Setup(x => x.CurrentUser())
            .Returns((Guid?)null);

        _localizationServiceMock.Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not authorized");

        var command = new UserLeftToEventCommand { EventId = Guid.NewGuid() };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Not authorized");
    }

    // =========================
    // ❌ Kullanıcı etkinliğe katılmamış
    // =========================
    [Fact]
    public async Task Handle_UserNotJoinedEvent_ShouldFail()
    {
        var userId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser())
            .Returns(userId);

        _currentUserServiceMock.Setup(x => x.Role())
            .Returns(Role.User);

        _eventUserRepositoryMock.Setup(x =>
            x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync((EventUser)null);

        _localizationServiceMock.Setup(x => x.Get(ValidationKeys.NotJoinedEvent))
            .ReturnsAsync("Not joined");

        var command = new UserLeftToEventCommand { EventId = Guid.NewGuid() };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Not joined");
    }

    // =========================
    // ✅ Başarılı ayrılma
    // =========================
    [Fact]
    public async Task Handle_ValidRequest_ShouldLeaveEventSuccessfully()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var eventUser = new EventUser
        {
            UserId = userId,
            EventId = eventId,
            IsJoined = true,
            User = new User { Email = "test@mail.com" }
        };

        var eventEntity = new UniTrack.Domain.Entities.Event
        {
            Id = eventId,
            Joiner = 5,
            SheetsId = "sheet123"
        };

        _currentUserServiceMock.Setup(x => x.CurrentUser())
            .Returns(userId);

        _currentUserServiceMock.Setup(x => x.Role())
            .Returns(Role.User);

        _eventUserRepositoryMock.Setup(x =>
            x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync(eventUser);

        _eventRepositoryMock.Setup(x =>
            x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UniTrack.Domain.Entities.Event, bool>>>()))
            .ReturnsAsync(eventEntity);

        _localizationServiceMock.Setup(x => x.Get(ValidationKeys.EventLeftSuccess))
            .ReturnsAsync("Left event");

        var command = new UserLeftToEventCommand { EventId = eventId };

        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Left event");

        _eventUserRepositoryMock.Verify(x => x.DeleteAsync(eventUser), Times.Once);
        _eventRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<UniTrack.Domain.Entities.Event>()), Times.Once);
        _participantSheetRepositoryMock.Verify(
            x => x.RemoveParticipantAsync("sheet123", "test@mail.com"),
            Times.Once);
    }
}
