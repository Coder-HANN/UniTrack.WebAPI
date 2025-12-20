using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
    public class GetActiveClubsFor180DaysQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServicesMock;
        private readonly Mock<IClubRepository> _clubRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly GetActiveClubsFor180DaysQueryHandler _handler;

        public GetActiveClubsFor180DaysQueryHandlerTests()
        {
            _currentUserServicesMock = new Mock<ICurrentUserServices>();
            _clubRepositoryMock = new Mock<IClubRepository>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new GetActiveClubsFor180DaysQueryHandler(
                _currentUserServicesMock.Object,
                _clubRepositoryMock.Object,
                _localizationServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserIsNotAdmin_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserServicesMock.Setup(x => x.Role())
                .Returns(Role.User);

            _localizationServiceMock.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Yetkisiz erişim");

            var query = new GetActiveClubsFor180DaysQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Equal("Yetkisiz erişim", result.Message);

            _clubRepositoryMock.Verify(
                x => x.Get180DaysActiveClubsCountAsync(),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_UserIsAdmin_ActiveClubCountGreaterThanZero_ShouldReturnCount()
        {
            // Arrange
            _currentUserServicesMock.Setup(x => x.Role())
                .Returns(Role.Admin);

            _clubRepositoryMock.Setup(x => x.Get180DaysActiveClubsCountAsync())
                .ReturnsAsync(5);

            var query = new GetActiveClubsFor180DaysQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Data);
            Assert.Null(result.Message);

            _clubRepositoryMock.Verify(
                x => x.Get180DaysActiveClubsCountAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_UserIsAdmin_ActiveClubCountIsZero_ShouldReturnZero()
        {
            // Arrange
            _currentUserServicesMock.Setup(x => x.Role())
                .Returns(Role.Admin);

            _clubRepositoryMock.Setup(x => x.Get180DaysActiveClubsCountAsync())
                .ReturnsAsync(0);

            var query = new GetActiveClubsFor180DaysQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data);
            Assert.Null(result.Message);
        }
    }
}
