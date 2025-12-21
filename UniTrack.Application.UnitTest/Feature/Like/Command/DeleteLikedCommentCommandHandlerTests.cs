using Xunit;
using Moq;
using FluentAssertions;
using UniTrack.Application.Feature.Like.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using System.Linq.Expressions;

namespace UniTrack.Application.Tests.Feature.Like.Command
{
    public class DeleteLikedCommentCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUser;
        private readonly Mock<ILikeRepository> _likeRepo;
        private readonly Mock<ICommentRepository> _commentRepo;
        private readonly Mock<ILocalizationService> _localization;

        private readonly DeleteLikedCommentCommandHandler _handler;

        public DeleteLikedCommentCommandHandlerTests()
        {
            _currentUser = new Mock<ICurrentUserServices>();
            _likeRepo = new Mock<ILikeRepository>();
            _commentRepo = new Mock<ICommentRepository>();
            _localization = new Mock<ILocalizationService>();

            _handler = new DeleteLikedCommentCommandHandler(
                _currentUser.Object,
                _likeRepo.Object,
                _commentRepo.Object,
                _localization.Object
            );
        }

        [Fact]
        public async Task Handle_NotAuthorized_ShouldReturnError()
        {
            _currentUser.Setup(x => x.CurrentUser()).Returns((Guid?)null);
            _currentUser.Setup(x => x.CurrentClub()).Returns((Guid?)null);

            _localization.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Not authorized");

            var result = await _handler.Handle(
                new DeleteLikedCommentCommand(),
                CancellationToken.None);

            result.Should().Be("Not authorized");
        }

        [Fact]
        public async Task Handle_LikeNotFound_ShouldReturnError()
        {
            var userId = Guid.NewGuid();

            _currentUser.Setup(x => x.CurrentUser()).Returns(userId);
            _currentUser.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());

            _likeRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Like, bool>>>()))
                .ReturnsAsync((Domain.Entities.Like)null);

            _localization.Setup(x => x.Get(ValidationKeys.CommentNotFound))
                .ReturnsAsync("Comment not found");

            var result = await _handler.Handle(
                new DeleteLikedCommentCommand { CommentId = Guid.NewGuid() },
                CancellationToken.None);

            result.Should().Be("Comment not found");
        }

        [Fact]
        public async Task Handle_Success_ShouldDeleteLike()
        {
            _currentUser.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUser.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());

            _likeRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Like, bool>>>()))
                .ReturnsAsync(new Domain.Entities.Like());

            _localization.Setup(x => x.Get(It.IsAny<string>()))
                .ReturnsAsync("Deleted");

            var result = await _handler.Handle(
                new DeleteLikedCommentCommand { CommentId = Guid.NewGuid() },
                CancellationToken.None);

            _likeRepo.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Like>()), Times.Once);
            result.Should().Be("Deleted");
        }
    }
}
