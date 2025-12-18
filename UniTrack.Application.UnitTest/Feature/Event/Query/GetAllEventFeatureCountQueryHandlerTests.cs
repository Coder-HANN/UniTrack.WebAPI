using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;

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
        public async Task Handle_FeatureEventCountIsZero_ShouldReturnFail()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetAllEventFeatureCountAsync())
                .ReturnsAsync(0);

            var query = new GetAllEventCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
        }

        [Fact]
        public async Task Handle_FeatureEventCountGreaterThanZero_ShouldReturnSuccess()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetAllEventFeatureCountAsync())
                .ReturnsAsync(10);

            var query = new GetAllEventCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10, result.Data);
        }
    }
}
