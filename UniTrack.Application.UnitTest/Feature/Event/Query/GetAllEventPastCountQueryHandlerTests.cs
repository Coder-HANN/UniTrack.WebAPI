using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;

namespace UniTrack.Application.Tests.Feature.Event.Query
{
    public class GetAllEventPastCountQueryHandlerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly GetAllEventPastCountQueryHandler _handler;

        public GetAllEventPastCountQueryHandlerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _handler = new GetAllEventPastCountQueryHandler(_eventRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_PastEventCountIsZero_ShouldReturnFail()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetAllPastEventCountAsync())
                .ReturnsAsync(0);

            var query = new GetAllEventPastCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
        }

        [Fact]
        public async Task Handle_PastEventCountGreaterThanZero_ShouldReturnSuccess()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetAllPastEventCountAsync())
                .ReturnsAsync(5);

            var query = new GetAllEventPastCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Data);
        }
    }
}
