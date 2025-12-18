using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Feature.Event.Query;

namespace UniTrack.Application.Tests.Feature.Event.Query
{
    public class GetAllEventCountQueryHandlerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly GetAllEventCountQueryHandler _handler;

        public GetAllEventCountQueryHandlerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _handler = new GetAllEventCountQueryHandler(_eventRepositoryMock.Object);
        }

        // ✅ EVENT VAR → SUCCESS = TRUE
        [Fact]
        public async Task Handle_WhenEventCountGreaterThanZero_ShouldReturnSuccess()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(5);

            var query = new GetAllEventCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Data);

            _eventRepositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
        }

        // ❌ EVENT YOK → SUCCESS = FALSE
        [Fact]
        public async Task Handle_WhenEventCountIsZero_ShouldReturnFail()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            var query = new GetAllEventCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);

            _eventRepositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
        }

        // ❌ NULL / HATALI DURUM
        [Fact]
        public async Task Handle_WhenEventCountIsNegative_ShouldReturnFail()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(-1);

            var query = new GetAllEventCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);

            _eventRepositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
        }
    }
}
