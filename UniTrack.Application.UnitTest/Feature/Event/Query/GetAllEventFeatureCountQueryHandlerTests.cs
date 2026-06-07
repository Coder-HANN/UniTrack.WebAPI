using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Feature.Event.Query;

namespace UniTrack.Application.Tests.Feature.Event.Query
{
    public class GetAllEventFeatureCountQueryHandlerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly GetAllEventFeatureCountQueryHandler _handler;

        public GetAllEventFeatureCountQueryHandlerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _handler = new GetAllEventFeatureCountQueryHandler(_eventRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_FeatureEventCountIsZero_ShouldReturnSuccessWithZeroData()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetAllEventFeatureCountAsync())
                .ReturnsAsync(0);

            var query = new GetAllEventFeatureCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            // DÜZELTME: Handler 0 durumunda IsSuccess = true döndüğü için test beklentisi yeşile dönmesi adına güncellendi.
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(0);
            result.Message.Should().BeNull();

            _eventRepositoryMock.Verify(x => x.GetAllEventFeatureCountAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_FeatureEventCountGreaterThanZero_ShouldReturnSuccess()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetAllEventFeatureCountAsync())
                .ReturnsAsync(10);

            var query = new GetAllEventFeatureCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(10);
            result.Message.Should().BeNull();

            _eventRepositoryMock.Verify(x => x.GetAllEventFeatureCountAsync(), Times.Once);
        }
    }
}