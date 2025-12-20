using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Feature.ActiveDetail.Query;
using Xunit;

namespace UniTrack.Application.Tests.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor30DaysQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetActiveUsersFor30DaysQueryHandler _handler;

        public GetActiveUsersFor30DaysQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetActiveUsersFor30DaysQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ActiveUsersGreaterThanZero_ShouldReturnSuccess()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.CountAsync()).ReturnsAsync(8);

            // Act
            var result = await _handler.Handle(new GetActiveUsersFor30DaysQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(8, result.Data);
            Assert.Null(result.Message);
        }

        [Fact]
        public async Task Handle_ActiveUsersZero_ShouldReturnFail()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.CountAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new GetActiveUsersFor30DaysQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Null(result.Message);
        }
    }
}
