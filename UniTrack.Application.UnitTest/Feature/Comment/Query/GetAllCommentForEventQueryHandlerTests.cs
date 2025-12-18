using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Feature.Comment.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Domain.Entities;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Tests.Feature.Comment.Query
{
    public class GetAllCommentForEventQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly GetAllCommentForEventQueryHandler _handler;

        public GetAllCommentForEventQueryHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserServices>();
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new GetAllCommentForEventQueryHandler(
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

            _localizationServiceMock.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Yetkisiz erişim");

            var query = new GetAllCommentForEventQuery(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Yetkisiz erişim");
        }

        [Fact]
        public async Task Handle_CommentsNotFound_ShouldReturnCommentNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _commentRepositoryMock.Setup(x => x.GetAllCommentByEventIdAsync(eventId))
                .ReturnsAsync(new List<Domain.Entities.Comment>());

            _localizationServiceMock.Setup(x => x.Get(ValidationKeys.CommentNotFound))
                .ReturnsAsync("Yorum bulunamadı");

            var query = new GetAllCommentForEventQuery(eventId);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Yorum bulunamadı");
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnCommentList()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var comments = new List<Domain.Entities.Comment>
            {
                new Domain.Entities.Comment
                {
                    Description = "Harika etkinlik",
                    Point = 5,
                    CreatedDate = DateTime.UtcNow,
                    User = new User
                    {
                        UserDetail = new UserDetail
                        {
                            Name = "Ali",
                            Surname = "Yılmaz"
                        }
                    }
                }
            };

            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _commentRepositoryMock.Setup(x => x.GetAllCommentByEventIdAsync(eventId))
                .ReturnsAsync(comments);

            var query = new GetAllCommentForEventQuery(eventId);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);

            var comment = result.Data.First();
            comment.Name.Should().Be("Ali");
            comment.Surname.Should().Be("Yılmaz");
            comment.Description.Should().Be("Harika etkinlik");
            comment.Point.Should().Be(5);
        }
    }
}
