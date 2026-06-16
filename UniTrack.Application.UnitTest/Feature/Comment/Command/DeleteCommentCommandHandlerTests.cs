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

    // --------------------------------------------------
    // ❌ COMMENT NOT FOUND
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Comment_NotFound()
    {
        // Arrange
        var commentId = Guid.NewGuid();

        _commentRepository
            .Setup(x => x.GetCommentIdAsync(commentId))
            .ReturnsAsync((UniTrack.Domain.Entities.Comment)null);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.CommentNotFound))
            .ReturnsAsync(ValidationKeys.CommentNotFound);

        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.CommentNotFound);

        // Delete must never be reached
        _commentRepository.Verify(
            x => x.DeleteCommentWithLikesAsync(It.IsAny<UniTrack.Domain.Entities.Comment>()),
            Times.Never);
    }

    // --------------------------------------------------
    // ❌ USER TRIES TO DELETE SOMEONE ELSE'S COMMENT
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_User_Is_Not_Authorized_To_Delete_Others_Comment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        // Comment belongs to a DIFFERENT user
        var comment = new UniTrack.Domain.Entities.Comment
        {
            Id = commentId,
            UserId = Guid.NewGuid()
        };

        _commentRepository
            .Setup(x => x.GetCommentIdAsync(commentId))
            .ReturnsAsync(comment);

        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid()); // different user

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync(ValidationKeys.NotAuthorized);

        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);

        _commentRepository.Verify(
            x => x.DeleteCommentWithLikesAsync(It.IsAny<UniTrack.Domain.Entities.Comment>()),
            Times.Never);
    }

    // --------------------------------------------------
    // ✅ ADMIN → DELETES ANY COMMENT
    // --------------------------------------------------
    // Handler calls DeleteCommentWithLikesAsync, NOT DeleteAsync.
    [Fact]
    public async Task Handle_Should_Delete_Comment_Successfully_When_User_Is_Admin()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new UniTrack.Domain.Entities.Comment { Id = commentId };

        _currentUserServices.Setup(x => x.Role()).Returns(Role.Admin);

        _commentRepository
            .Setup(x => x.GetCommentIdAsync(commentId))
            .ReturnsAsync(comment);

        // ✅ Correct method: DeleteCommentWithLikesAsync (not DeleteAsync)
        _commentRepository
            .Setup(x => x.DeleteCommentWithLikesAsync(comment))
            .Returns(Task.CompletedTask);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.CommentDeleted))
            .ReturnsAsync(ValidationKeys.CommentDeleted);

        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ValidationKeys.CommentDeleted);

        _commentRepository.Verify(x => x.DeleteCommentWithLikesAsync(comment), Times.Once);
        _commentRepository.Verify(x => x.DeleteAsync(It.IsAny<UniTrack.Domain.Entities.Comment>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ USER OWNS THE COMMENT → CAN DELETE
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Delete_Comment_Successfully_When_User_Owns_The_Comment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var comment = new UniTrack.Domain.Entities.Comment { Id = commentId, UserId = userId };

        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);

        _commentRepository
            .Setup(x => x.GetCommentIdAsync(commentId))
            .ReturnsAsync(comment);

        // ✅ Correct method: DeleteCommentWithLikesAsync (not DeleteAsync)
        _commentRepository
            .Setup(x => x.DeleteCommentWithLikesAsync(comment))
            .Returns(Task.CompletedTask);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.CommentDeleted))
            .ReturnsAsync(ValidationKeys.CommentDeleted);

        var command = new DeleteCommentCommand { CommentId = commentId };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ValidationKeys.CommentDeleted);

        _commentRepository.Verify(x => x.DeleteCommentWithLikesAsync(comment), Times.Once);
        _commentRepository.Verify(x => x.DeleteAsync(It.IsAny<UniTrack.Domain.Entities.Comment>()), Times.Never);
    }
}