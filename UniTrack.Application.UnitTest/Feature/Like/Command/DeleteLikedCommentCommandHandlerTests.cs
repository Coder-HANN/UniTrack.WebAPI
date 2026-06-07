using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using UniTrack.Application.Feature.Like.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common; // 💡 ServiceResponse sınıfını tanımak için kritik using
using UniTrack.Application.Common.Constants;

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
            // Arrange
            _currentUser.Setup(x => x.CurrentUser()).Returns((Guid?)null);
            _currentUser.Setup(x => x.CurrentClub()).Returns((Guid?)null);

            _localization.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Not authorized");

            // Act
            var result = await _handler.Handle(
                new DeleteLikedCommentCommand(),
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Not authorized");
        }

        [Fact]
        public async Task Handle_LikeNotFound_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUser.Setup(x => x.CurrentUser()).Returns(userId);
            _currentUser.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());

            _likeRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Like, bool>>>()))
                .ReturnsAsync((UniTrack.Domain.Entities.Like)null);

            _localization.Setup(x => x.Get(ValidationKeys.CommentNotFound))
                .ReturnsAsync("Comment not found");

            // Act
            var result = await _handler.Handle(
                new DeleteLikedCommentCommand { CommentId = Guid.NewGuid() },
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Comment not found");
        }

        [Fact]
        public async Task Handle_Success_ShouldDeleteLike()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _currentUser.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUser.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());

            _likeRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UniTrack.Domain.Entities.Like, bool>>>()))
                .ReturnsAsync(new UniTrack.Domain.Entities.Like());

            _localization.Setup(x => x.Get("Beğeni silindi"))
                .ReturnsAsync("Beğeni silindi");

            // Act
            var result = await _handler.Handle(
                new DeleteLikedCommentCommand { CommentId = commentId },
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Beğeni silindi");

            // Veritabanından beğeninin kaldırıldığını doğrula
            _likeRepo.Verify(x => x.DeleteAsync(It.IsAny<UniTrack.Domain.Entities.Like>()), Times.Once);

            // Yorumun beğeni sayısının 1 azaltıldığını doğrula (Decrement)
            _commentRepo.Verify(x => x.DecrementLikeCountAsync(commentId), Times.Once);
        }
    }
}