using FluentAssertions;
using Moq;
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
    public async Task Handle_Should_Fail_When_User_Is_Not_Admin()
    {
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();
        var command = new DeleteCommentCommand { CommentId = Guid.NewGuid() };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Delete_Comment_Successfully_When_User_Is_Admin()
    {
        var commentId = Guid.NewGuid();
        var comment = new Domain.Entities.Comment { Id = commentId };

        _currentUserServices.Setup(x => x.Role()).Returns(Role.Admin);
        _commentRepository.Setup(x => x.GetCommentIdAsync(commentId)).ReturnsAsync(comment);
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
