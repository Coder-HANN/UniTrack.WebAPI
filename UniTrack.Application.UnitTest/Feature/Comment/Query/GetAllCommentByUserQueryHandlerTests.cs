using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Feature.Comment.Query;
using UniTrack.Domain.Enums;
using Xunit;

namespace UniTrack.Application.Tests.Feature.Comment.Query
{
    public class GetAllCommentByUserQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly GetAllCommentByUserQueryHandler _handler;

        public GetAllCommentByUserQueryHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserServices>();
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new GetAllCommentByUserQueryHandler(
                _currentUserServiceMock.Object,
                _commentRepositoryMock.Object,
                _localizationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns((Guid?)null);

            _localizationServiceMock.Setup(x => x.Get(It.IsAny<string>()))
                .ReturnsAsync("Yetkisiz erişim");

            // Act
            var result = await _handler.Handle(new GetAllCommentByUserQuery(), default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Yetkisiz erişim");
        }

        [Fact]
        public async Task Handle_UserRoleIsClub_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.Club);

            _localizationServiceMock.Setup(x => x.Get(It.IsAny<string>()))
                .ReturnsAsync("Yetkisiz erişim");

            // Act
            var result = await _handler.Handle(new GetAllCommentByUserQuery(), default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_CommentsNotFound_ShouldReturnCommentNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(userId);

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.User);

            _commentRepositoryMock.Setup(x => x.GetAllCommentsByUserIdAsync(userId))
                .ReturnsAsync((List<Domain.Entities.Comment>)null);

            _localizationServiceMock.Setup(x => x.Get(It.IsAny<string>()))
                .ReturnsAsync("Yorum bulunamadı");

            // Act
            var result = await _handler.Handle(new GetAllCommentByUserQuery(), default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Yorum bulunamadı");
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnCommentList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var comments = new List<Domain.Entities.Comment>
            {
                new Domain.Entities.Comment
                {
                    Id = Guid.NewGuid(),
                    EventId = Guid.NewGuid(),
                    ClubId = Guid.NewGuid(),
                    Description = "Test yorum",
                    Point = 5
                }
            };

            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(userId);

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.User);

            _commentRepositoryMock.Setup(x => x.GetAllCommentsByUserIdAsync(userId))
                .ReturnsAsync(comments);

            // Act
            var result = await _handler.Handle(new GetAllCommentByUserQuery(), default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data.First().CommentText.Should().Be("Test yorum");
            result.Data.First().Point.Should().Be(5);
        }
    }
}
