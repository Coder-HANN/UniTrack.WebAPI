using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Feature.ActiveDetail.Query;
using Xunit;

namespace UniTrack.Application.Tests.Feature.ActiveDetail.Query
{
    public class GetUserCountQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserCountQueryHandler _handler;

        public GetUserCountQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserCountQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTotalUserCount()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetUserCountAsync())
                .ReturnsAsync(42);

            // Act
            var result = await _handler.Handle(new GetUserCountQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(42, result);
            _userRepositoryMock.Verify(x => x.GetUserCountAsync(), Times.Once);
        }
    }
}
