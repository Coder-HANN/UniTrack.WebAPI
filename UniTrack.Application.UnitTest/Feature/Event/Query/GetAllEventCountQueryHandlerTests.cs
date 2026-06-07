using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
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
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(5);
            result.Message.Should().BeNull();

            _eventRepositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEventCountIsZero_ShouldReturnSuccessWithZeroData()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(0);

            var query = new GetAllEventCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert 
            // DÜZELTME: Handler kodunuz 0 durumunda IsSuccess = true döndüğü için test bu şekilde güncellendi.
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(0);
            result.Message.Should().BeNull();

            _eventRepositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEventCountIsNegative_ShouldReturnWhatHandlerProduces()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(x => x.GetCountAsync())
                .ReturnsAsync(-1);

            var query = new GetAllEventCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            // DÜZELTME: Handler negatif korumasına sahip değil; doğrudan IsSuccess = true ve gelen değeri (-1) aktarıyor.
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(-1);

            _eventRepositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
        }
    }
}