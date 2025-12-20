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
    public class GetActiveClubsFor30DaysQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserMock;
        private readonly Mock<IClubRepository> _clubRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationMock;

        private readonly GetActiveClubsFor30DaysQueryHandler _handler;

        public GetActiveClubsFor30DaysQueryHandlerTests()
        {
            _currentUserMock = new Mock<ICurrentUserServices>();
            _clubRepositoryMock = new Mock<IClubRepository>();
            _localizationMock = new Mock<ILocalizationService>();

            _handler = new GetActiveClubsFor30DaysQueryHandler(
                _currentUserMock.Object,
                _clubRepositoryMock.Object,
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
            var result = await _handler.Handle(new GetActiveClubsFor30DaysQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Equal("Not Authorized", result.Message);

            _clubRepositoryMock.Verify(
                x => x.Get30DaysActiveClubsCountAsync(),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_AdminUser_ActiveClubsGreaterThanZero_ShouldReturnSuccess()
        {
            // Arrange
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);
            _clubRepositoryMock.Setup(x => x.Get30DaysActiveClubsCountAsync())
                .ReturnsAsync(4);

            // Act
            var result = await _handler.Handle(new GetActiveClubsFor30DaysQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(4, result.Data);
            Assert.Null(result.Message);
        }

        [Fact]
        public async Task Handle_AdminUser_ActiveClubsZero_ShouldReturnFail()
        {
            // Arrange
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);
            _clubRepositoryMock.Setup(x => x.Get30DaysActiveClubsCountAsync())
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new GetActiveClubsFor30DaysQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Null(result.Message);
        }
    }
}
