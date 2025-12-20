using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.ActiveDetail.Query;
using UniTrack.Domain.Enums;
using Xunit;

namespace UniTrack.Application.Tests.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor90DaysQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICurrentUserServices> _currentUserMock;
        private readonly Mock<ILocalizationService> _localizationMock;

        private readonly GetActiveUsersFor90DaysQueryHandler _handler;

        public GetActiveUsersFor90DaysQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _currentUserMock = new Mock<ICurrentUserServices>();
            _localizationMock = new Mock<ILocalizationService>();

            _handler = new GetActiveUsersFor90DaysQueryHandler(
                _userRepositoryMock.Object,
                _currentUserMock.Object,
                _localizationMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserIsNotAdmin_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserMock.Setup(x => x.Role()).Returns(Role.User);
            _localizationMock.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Not Authorized");

            // Act
            var result = await _handler.Handle(new GetActiveUsersFor90DaysQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Equal("Not Authorized", result.Message);

            _userRepositoryMock.Verify(x => x.For90DaysCountAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_AdminUser_ActiveUsersGreaterThanZero_ShouldReturnCount()
        {
            // Arrange
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);
            _userRepositoryMock.Setup(x => x.For90DaysCountAsync()).ReturnsAsync(18);

            // Act
            var result = await _handler.Handle(new GetActiveUsersFor90DaysQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(18, result.Data);
            Assert.Null(result.Message);
        }

        [Fact]
        public async Task Handle_AdminUser_ActiveUsersZero_ShouldReturnFail()
        {
            // Arrange
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);
            _userRepositoryMock.Setup(x => x.For90DaysCountAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new GetActiveUsersFor90DaysQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Null(result.Message);
        }
    }
}
