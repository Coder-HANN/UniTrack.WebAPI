using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Feature.Event.Query;

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
        public async Task Handle_PastEventCountIsZero_ShouldReturnSuccessWithZeroData()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetAllPastEventCountAsync())
                .ReturnsAsync(0);

            var query = new GetAllEventPastCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            // DÜZELTME: Handler 0 veya null durumlarında IsSuccess = true döndüğü için test kurgusu düzeltildi.
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(0);
            result.Message.Should().BeNull();

            _eventRepositoryMock.Verify(x => x.GetAllPastEventCountAsync(), Times.Once);
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
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(5);
            result.Message.Should().BeNull();

            _eventRepositoryMock.Verify(x => x.GetAllPastEventCountAsync(), Times.Once);
        }
    }
}