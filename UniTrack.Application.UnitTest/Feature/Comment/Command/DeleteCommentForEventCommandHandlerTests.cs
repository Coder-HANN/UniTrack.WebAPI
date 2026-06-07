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
using UniTrack.Domain.Enums;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;

public class DeleteCommentForEventCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private DeleteCommentCommandHandler CreateHandler()
        => new(_currentUserServices.Object,
               _commentRepository.Object,
               _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Fail_When_User_Is_Not_LoggedIn()
    {
        _currentUserServices.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = Guid.NewGuid() };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_User_Is_Club()
    {
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = Guid.NewGuid() };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Comment_Not_Found()
    {
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _commentRepository.Setup(x => x.GetCommentByEventAndUserIdAsync(commentId, userId))
                          .ReturnsAsync((Comment)null);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.CommentNotFound);
    }

    [Fact]
    public async Task Handle_Should_Delete_Comment_Successfully()
    {
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, UserId = userId };

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _commentRepository.Setup(x => x.GetCommentByEventAndUserIdAsync(commentId, userId))
                          .ReturnsAsync(comment);
        _commentRepository.Setup(x => x.DeleteAsync(comment)).Returns(Task.CompletedTask);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ValidationKeys.CommentDeleted);
        _commentRepository.Verify(x => x.DeleteAsync(comment), Times.Once);
    }
}
