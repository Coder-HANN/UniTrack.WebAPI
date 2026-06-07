using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Comment.Command;
using UniTrack.Domain.Enums;
using Xunit;

public class DeleteCommentCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private DeleteCommentCommandHandler CreateHandler()
        => new(_currentUserServices.Object,
               _commentRepository.Object,
               _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Fail_When_Comment_NotFound()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId))
            .ReturnsAsync((UniTrack.Domain.Entities.Comment)null);

        _localizationService.Setup(x => x.Get(ValidationKeys.CommentNotFound))
            .ReturnsAsync(ValidationKeys.CommentNotFound);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.CommentNotFound);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_User_Is_Not_Authorized_To_Delete_Others_Comment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new UniTrack.Domain.Entities.Comment { Id = commentId, UserId = Guid.NewGuid() }; // Farklı bir kullanıcıya ait yorum

        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid()); // Farklı bir oturum açmış kullanıcı ID'si

        _localizationService.Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync(ValidationKeys.NotAuthorized);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Delete_Comment_Successfully_When_User_Is_Admin()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new UniTrack.Domain.Entities.Comment { Id = commentId };

        _currentUserServices.Setup(x => x.Role()).Returns(Role.Admin);
        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
        _commentRepository.Setup(x => x.DeleteAsync(comment)).Returns(Task.CompletedTask);
        _localizationService.Setup(x => x.Get(ValidationKeys.CommentDeleted))
            .ReturnsAsync(ValidationKeys.CommentDeleted);

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
    public async Task Handle_Should_Delete_Comment_Successfully_When_User_Owns_The_Comment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment = new UniTrack.Domain.Entities.Comment { Id = commentId, UserId = userId };

        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);

        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
        _commentRepository.Setup(x => x.DeleteAsync(comment)).Returns(Task.CompletedTask);
        _localizationService.Setup(x => x.Get(ValidationKeys.CommentDeleted))
            .ReturnsAsync(ValidationKeys.CommentDeleted);

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