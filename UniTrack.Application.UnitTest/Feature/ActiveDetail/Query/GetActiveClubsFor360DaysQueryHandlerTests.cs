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
    public class GetActiveClubsFor360DaysQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserMock;
        private readonly Mock<IClubRepository> _clubRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationMock;

        private readonly GetActiveClubsFor360DaysQueryHandler _handler;

        public GetActiveClubsFor360DaysQueryHandlerTests()
        {
            _currentUserMock = new Mock<ICurrentUserServices>();
            _clubRepositoryMock = new Mock<IClubRepository>();
            _localizationMock = new Mock<ILocalizationService>();

            _handler = new GetActiveClubsFor360DaysQueryHandler(
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
            var result = await _handler.Handle(new GetActiveClubsFor360DaysQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Equal("Not Authorized", result.Message);

            _clubRepositoryMock.Verify(
                x => x.Get360DaysActiveClubsCountAsync(),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_AdminUser_ActiveClubsGreaterThanZero_ShouldReturnCount()
        {
            // Arrange
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);
            _clubRepositoryMock.Setup(x => x.Get360DaysActiveClubsCountAsync())
                .ReturnsAsync(12);

            // Act
            var result = await _handler.Handle(new GetActiveClubsFor360DaysQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(12, result.Data);
            Assert.Null(result.Message);
        }

        [Fact]
        public async Task Handle_AdminUser_ActiveClubsZeroOrLess_ShouldReturnZero()
        {
            // Arrange
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Admin);
            _clubRepositoryMock.Setup(x => x.Get360DaysActiveClubsCountAsync())
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new GetActiveClubsFor360DaysQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Null(result.Message);
        }
    }
}
