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
    public async Task Handle_Should_Fail_When_User_Is_Not_LoggedIn_Or_Not_Authorized()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, UserId = Guid.NewGuid() }; // Başka bir kullanıcıya ait yorum

        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
        _currentUserServices.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _localizationService.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync(ValidationKeys.NotAuthorized);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Club_Tries_To_Delete_Others_Comment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var myClubId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, ClubId = Guid.NewGuid() }; // Başka bir kulübe ait yorum

        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
        _currentUserServices.Setup(x => x.CurrentClub()).Returns(myClubId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);
        _localizationService.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync(ValidationKeys.NotAuthorized);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Comment_Not_Found()
    {
        // Arrange
        var commentId = Guid.NewGuid();

        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync((Comment)null);
        _localizationService.Setup(x => x.Get(ValidationKeys.CommentNotFound)).ReturnsAsync(ValidationKeys.CommentNotFound);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.CommentNotFound);
    }

    [Fact]
    public async Task Handle_Should_Delete_Comment_Successfully_When_User_Owns_The_Comment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, UserId = userId };

        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _commentRepository.Setup(x => x.DeleteAsync(comment)).Returns(Task.CompletedTask);
        _localizationService.Setup(x => x.Get(ValidationKeys.CommentDeleted)).ReturnsAsync(ValidationKeys.CommentDeleted);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ValidationKeys.CommentDeleted);
        _commentRepository.Verify(x => x.DeleteAsync(comment), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Delete_Comment_Successfully_When_User_Is_Admin()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, UserId = Guid.NewGuid() }; // Herhangi bir yorum

        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Admin);
        _commentRepository.Setup(x => x.DeleteAsync(comment)).Returns(Task.CompletedTask);
        _localizationService.Setup(x => x.Get(ValidationKeys.CommentDeleted)).ReturnsAsync(ValidationKeys.CommentDeleted);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ValidationKeys.CommentDeleted);
        _commentRepository.Verify(x => x.DeleteAsync(comment), Times.Once);
    }
}