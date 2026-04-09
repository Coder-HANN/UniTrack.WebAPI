using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Application.Common.Constants;

public class UserJoinToEventCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<IUserDetailRepository> _userDetailRepositoryMock;
    private readonly Mock<IParticipantSheetRepository> _sheetRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;

    private readonly UserJoinToEventCommandHandler _handler;

    public UserJoinToEventCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserServices>();
        _eventUserRepositoryMock = new Mock<IEventUserRepository>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _userDetailRepositoryMock = new Mock<IUserDetailRepository>();
        _sheetRepositoryMock = new Mock<IParticipantSheetRepository>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _handler = new UserJoinToEventCommandHandler(
            _currentUserServiceMock.Object,
            _eventUserRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _userDetailRepositoryMock.Object,
            _sheetRepositoryMock.Object,
            _localizationServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_User_Joins_Event_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

        var eventEntity = new Event
        {
            Id = eventId,
            EndDate = DateTime.UtcNow.AddDays(1),
            Quota = 10,
            Joiner = 3,
            Status = Status.Public,
            SheetsId = "sheet-id",
            Club = new Club { UniversityId = Guid.NewGuid() }
        };

        _eventRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Event, bool>>>()))
            .ReturnsAsync(eventEntity);

        _eventUserRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync((EventUser)null);

        var userDetail = new UserDetail
        {
            UserId = userId,
            Name = "Ali",
            Surname = "Veli",
            UniverstiyId = eventEntity.Club.UniversityId,
            User = new User { Email = "ali@unitrack.com" },
            University = new University { Name = "Uni" },
            Department = new Department { Name = "CS" }
        };

        _userDetailRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserDetail, bool>>>()))
            .ReturnsAsync(userDetail);

        _localizationServiceMock
            .Setup(x => x.Get(ValidationKeys.EventJoinSuccess))
            .ReturnsAsync("Joined");

        // Act
        var result = await _handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None
        );

        // Assert
        Assert.True(result.IsSuccess);

        _eventUserRepositoryMock.Verify(
            x => x.AddAsync(It.Is<EventUser>(eu =>
                eu.EventId == eventId &&
                eu.UserId == userId &&
                eu.IsJoined == true)),
            Times.Once);

        _eventRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<Event>(e => e.Joiner == 4)),
            Times.Once);

        _sheetRepositoryMock.Verify(
            x => x.AddParticipantAsync(
                "sheet-id",
                It.IsAny<SheetParticipantDTO>()),
            Times.Once);
    }
}
