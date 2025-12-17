using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Feature.Comment.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using System.Linq.Expressions;

public class CreateCommentForEventCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<IEventUserRepository> _eventUserRepository = new();
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<IEventRepository> _eventRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private CreateCommentForEventCommandHandler CreateHandler()
        => new(_currentUserServices.Object,
               _eventUserRepository.Object,
               _commentRepository.Object,
               _eventRepository.Object,
               _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Fail_When_User_Is_Null()
    {
        _currentUserServices.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new CreateCommentForEventCommand { EventId = Guid.NewGuid(), Point = 5 };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Role_Is_Club()
    {
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new CreateCommentForEventCommand { EventId = Guid.NewGuid(), Point = 5 };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Event_Not_Found()
    {
        var userId = Guid.NewGuid();
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        _eventRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Event, bool>>>())).ReturnsAsync((Event)null);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new CreateCommentForEventCommand { EventId = Guid.NewGuid(), Point = 5 };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.EventNotFound);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Event_Not_Finished()
    {
        var userId = Guid.NewGuid();
        var evt = new Event
        {
            Id = Guid.NewGuid(),
            EndDate = DateTime.UtcNow.AddHours(1)
        };

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _eventRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Event, bool>>>())).ReturnsAsync(evt);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new CreateCommentForEventCommand { EventId = evt.Id, Point = 5 };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.EventNotFinished);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Already_Commented()
    {
        var userId = Guid.NewGuid();
        var evt = new Event { Id = Guid.NewGuid(), EndDate = DateTime.UtcNow.AddHours(-1) };

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _eventRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Event, bool>>>())).ReturnsAsync(evt);
        _commentRepository.Setup(x => x.GetCommentByEventAndUserIdAsync(evt.Id, userId)).ReturnsAsync(new Comment());
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new CreateCommentForEventCommand { EventId = evt.Id, Point = 5 };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.AlreadyCommented);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_User_Not_Joined_Event()
    {
        var userId = Guid.NewGuid();
        var evt = new Event { Id = Guid.NewGuid(), EndDate = DateTime.UtcNow.AddHours(-1) };

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _eventRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Event, bool>>>())).ReturnsAsync(evt);
        _commentRepository.Setup(x => x.GetCommentByEventAndUserIdAsync(evt.Id, userId)).ReturnsAsync((Comment)null);
        _eventUserRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<EventUser, bool>>>())).ReturnsAsync((EventUser)null);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new CreateCommentForEventCommand { EventId = evt.Id, Point = 5 };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.UserNotJoinedEvent);
    }

    [Fact]
    public async Task Handle_Should_Create_Comment_Successfully()
    {
        var userId = Guid.NewGuid();
        var evt = new Event { Id = Guid.NewGuid(), EndDate = DateTime.UtcNow.AddHours(-1), ClubId = Guid.NewGuid() };

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _eventRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Event, bool>>>())).ReturnsAsync(evt);
        _commentRepository.Setup(x => x.GetCommentByEventAndUserIdAsync(evt.Id, userId)).ReturnsAsync((Comment)null);
        _eventUserRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<EventUser, bool>>>())).ReturnsAsync(new EventUser { EventId = evt.Id, UserId = userId, IsJoined = true });
        _commentRepository.Setup(x => x.AddAsync(It.IsAny<Comment>())).ReturnsAsync((Comment c) => c);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new CreateCommentForEventCommand { EventId = evt.Id, Point = 5, Descripiton = "Harika etkinlik!" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ValidationKeys.CommentCreatedSuccess);
    }
}
