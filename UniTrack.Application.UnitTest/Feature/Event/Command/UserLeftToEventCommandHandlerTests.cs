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
}
