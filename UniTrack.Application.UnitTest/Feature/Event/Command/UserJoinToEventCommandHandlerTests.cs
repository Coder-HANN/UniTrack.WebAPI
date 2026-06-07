using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class UserJoinToEventCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<IUserDetailRepository> _userDetailRepositoryMock;
    private readonly Mock<IParticipantSheetRepository> _sheetRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;

    public UserJoinToEventCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserServices>();
        _eventUserRepositoryMock = new Mock<IEventUserRepository>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _userDetailRepositoryMock = new Mock<IUserDetailRepository>();
        _sheetRepositoryMock = new Mock<IParticipantSheetRepository>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _localizationServiceMock
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync((string key) => key);
    }

    private UserJoinToEventCommandHandler CreateHandler()
        => new UserJoinToEventCommandHandler(
            _currentUserServiceMock.Object,
            _eventUserRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _userDetailRepositoryMock.Object,
            _sheetRepositoryMock.Object,
            _localizationServiceMock.Object);

    [Fact]
    public async Task Handle_Should_Return_Success_When_User_Joins_Event_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var universityId = Guid.NewGuid();

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
            Club = new Club { UniversityId = universityId }
        };

        _eventRepositoryMock
            .Setup(x => x.GetEventIdAsync(eventId))
            .ReturnsAsync(eventEntity);

        _eventUserRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync((EventUser)null);

        var userDetail = new UserDetail
        {
            UserId = userId,
            Name = "Ali",
            Surname = "Veli",
            UniverstiyId = universityId,
            User = new User { Email = "ali@unitrack.com" },
            University = new University { Name = "Uni" },
            Department = new Department { Name = "CS" }
        };

        _userDetailRepositoryMock
            .Setup(x => x.GetUserForJoinAsync(userId))
            .ReturnsAsync(userDetail);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _eventUserRepositoryMock.Verify(
            x => x.AddAsync(It.Is<EventUser>(eu =>
                eu.EventId == eventId &&
                eu.UserId == userId &&
                eu.IsJoined == true &&
                eu.IsCheckedIn == false)),
            Times.Once);

        _eventRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Event>()),
            Times.Once);

        _sheetRepositoryMock.Verify(
            x => x.AddParticipantAsync("sheet-id", It.IsAny<SheetParticipantDTO>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_User_Not_Logged_In()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = Guid.NewGuid() },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventRepositoryMock.Verify(
            x => x.GetEventIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_User_Role_Is_Not_User()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.Admin);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = Guid.NewGuid() },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventRepositoryMock.Verify(
            x => x.GetEventIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Event_Not_Found()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

        _eventRepositoryMock
            .Setup(x => x.GetEventIdAsync(eventId))
            .ReturnsAsync((Event)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventUserRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<EventUser>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Event_Is_Expired()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

        _eventRepositoryMock
            .Setup(x => x.GetEventIdAsync(eventId))
            .ReturnsAsync(new Event
            {
                Id = eventId,
                EndDate = DateTime.UtcNow.AddDays(-1),
                Status = Status.Public,
                Club = new Club { UniversityId = Guid.NewGuid() }
            });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventUserRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<EventUser>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_User_Already_Joined()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

        _eventRepositoryMock
            .Setup(x => x.GetEventIdAsync(eventId))
            .ReturnsAsync(new Event
            {
                Id = eventId,
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Public,
                Club = new Club { UniversityId = Guid.NewGuid() }
            });

        _eventUserRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync(new EventUser { EventId = eventId, UserId = userId, IsJoined = true });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventUserRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<EventUser>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Quota_Is_Full()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

        _eventRepositoryMock
            .Setup(x => x.GetEventIdAsync(eventId))
            .ReturnsAsync(new Event
            {
                Id = eventId,
                EndDate = DateTime.UtcNow.AddDays(1),
                Quota = 10,
                Joiner = 10,
                Status = Status.Public,
                Club = new Club { UniversityId = Guid.NewGuid() }
            });

        _eventUserRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync((EventUser)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventUserRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<EventUser>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Event_University_Only_And_User_Different_University()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

        _eventRepositoryMock
            .Setup(x => x.GetEventIdAsync(eventId))
            .ReturnsAsync(new Event
            {
                Id = eventId,
                EndDate = DateTime.UtcNow.AddDays(1),
                Quota = 10,
                Joiner = 3,
                Status = Status.Private,
                Club = new Club { UniversityId = Guid.NewGuid() }
            });

        _eventUserRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync((EventUser)null);

        _userDetailRepositoryMock
            .Setup(x => x.GetUserForJoinAsync(userId))
            .ReturnsAsync(new UserDetail { UserId = userId, UniverstiyId = Guid.NewGuid() });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _eventUserRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<EventUser>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Not_Call_Sheet_When_SheetsId_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var universityId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

        _eventRepositoryMock
            .Setup(x => x.GetEventIdAsync(eventId))
            .ReturnsAsync(new Event
            {
                Id = eventId,
                EndDate = DateTime.UtcNow.AddDays(1),
                Quota = 10,
                Joiner = 3,
                Status = Status.Public,
                SheetsId = null,
                Club = new Club { UniversityId = universityId }
            });

        _eventUserRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EventUser, bool>>>()))
            .ReturnsAsync((EventUser)null);

        _userDetailRepositoryMock
            .Setup(x => x.GetUserForJoinAsync(userId))
            .ReturnsAsync(new UserDetail
            {
                UserId = userId,
                Name = "Ali",
                Surname = "Veli",
                UniverstiyId = universityId,
                User = new User { Email = "ali@unitrack.com" },
                University = new University { Name = "Uni" },
                Department = new Department { Name = "CS" }
            });


        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UserJoinToEventCommand { EventId = eventId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _sheetRepositoryMock.Verify(
            x => x.AddParticipantAsync(It.IsAny<string>(), It.IsAny<SheetParticipantDTO>()),
            Times.Never);
    }
}