using Xunit;
using Moq;
using FluentAssertions;
using UniTrack.Application.Feature.Comment.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Domain.Entities;
using UniTrack.Application.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UniTrack.Application.Tests.Feature.Comment.Query
{
    public class GetAllCommentForEventQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock = new();
        private readonly Mock<ICommentRepository> _commentRepositoryMock = new();
        private readonly Mock<ILocalizationService> _localizationServiceMock = new();
        private readonly GetAllCommentForEventQueryHandler _handler;

        public GetAllCommentForEventQueryHandlerTests()
        {
            _handler = new GetAllCommentForEventQueryHandler(
                _currentUserServiceMock.Object,
                _commentRepositoryMock.Object,
                _localizationServiceMock.Object);
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

            _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _commentRepositoryMock
                .Setup(x => x.GetAllCommentByEventIdAsync(eventId))
                .ReturnsAsync(comments);

            var query = new GetAllCommentForEventQuery(eventId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

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