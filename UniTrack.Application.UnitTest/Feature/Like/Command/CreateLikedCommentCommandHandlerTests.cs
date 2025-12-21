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
    public class CreateLikedCommentCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUser;
        private readonly Mock<ILocalizationService> _localization;
        private readonly Mock<ICommentRepository> _commentRepo;
        private readonly Mock<ILikeRepository> _likeRepo;

        private readonly CreateLikedCommentCommandHandler _handler;

        public CreateLikedCommentCommandHandlerTests()
        {
            _currentUser = new Mock<ICurrentUserServices>();
            _localization = new Mock<ILocalizationService>();
            _commentRepo = new Mock<ICommentRepository>();
            _likeRepo = new Mock<ILikeRepository>();

            _handler = new CreateLikedCommentCommandHandler(
                _currentUser.Object,
                _localization.Object,
                _commentRepo.Object,
                _likeRepo.Object
            );
        }

        [Fact]
        public async Task Handle_NotAuthorized_ShouldReturnError()
        {
            _currentUser.Setup(x => x.CurrentUser()).Returns((Guid?)null);
            _currentUser.Setup(x => x.CurrentClub()).Returns((Guid?)null);

            _localization.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Not authorized");

            var result = await _handler.Handle(new CreateLikedCommentCommand(), CancellationToken.None);

            result.Should().Be("Not authorized");
        }

        [Fact]
        public async Task Handle_CommentNotFound_ShouldReturnError()
        {
            _currentUser.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

            _commentRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Comment, bool>>>()))
                .ReturnsAsync((Domain.Entities.Comment)null);

            _localization.Setup(x => x.Get(ValidationKeys.CommentNotFound))
                .ReturnsAsync("Comment not found");

            var result = await _handler.Handle(
                new CreateLikedCommentCommand { CommentId = Guid.NewGuid() },
                CancellationToken.None);

            result.Should().Be("Comment not found");
        }

        [Fact]
        public async Task Handle_AlreadyLiked_ShouldReturnError()
        {
            _currentUser.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

            _commentRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Comment, bool>>>()))
                .ReturnsAsync(new Domain.Entities.Comment());

            _likeRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Like, bool>>>()))
                .ReturnsAsync(new Domain.Entities.Like());

            _localization.Setup(x => x.Get(ValidationKeys.AlreadyLiked))
                .ReturnsAsync("Already liked");

            var result = await _handler.Handle(
                new CreateLikedCommentCommand { CommentId = Guid.NewGuid() },
                CancellationToken.None);

            result.Should().Be("Already liked");
        }

        [Fact]
        public async Task Handle_Success_ShouldAddLikeAndIncrementCount()
        {
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            _currentUser.Setup(x => x.CurrentUser()).Returns(userId);

            _commentRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Comment, bool>>>()))
                .ReturnsAsync(new Domain.Entities.Comment());

            _likeRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Domain.Entities.Like, bool>>>()))
                .ReturnsAsync((Domain.Entities.Like)null);

            _localization.Setup(x => x.Get(It.IsAny<string>()))
                .ReturnsAsync("Success");

            var result = await _handler.Handle(
                new CreateLikedCommentCommand { CommentId = commentId },
                CancellationToken.None);

            _likeRepo.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Like>()), Times.Once);
            _commentRepo.Verify(x => x.IncrementLikeCountAsync(commentId), Times.Once);

            result.Should().Be("Success");
        }
    }
}
