using Xunit;
using Moq;
using FluentAssertions;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Tests.Feature.Event.Query
{
    public class GetAllClubEventJoinerCountQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly GetAllClubEventJoinerCountQueryHandler _handler;

        public GetAllClubEventJoinerCountQueryHandlerTests()
        {
            _currentUserMock = new Mock<ICurrentUserServices>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new GetAllClubEventJoinerCountQueryHandler(
                _currentUserMock.Object,
                _eventRepositoryMock.Object,
                _localizationServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserNotAuthorized_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);
            _currentUserMock.Setup(x => x.Role()).Returns(Role.User);

            _localizationServiceMock
                .Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Not authorized");

            var query = new GetAllClubEventJoinerCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(0);
            result.Message.Should().Be("Not authorized");
        }

        [Fact]
        public async Task Handle_ClubHasJoiners_ShouldReturnJoinerCount()
        {
            // Arrange
            var clubId = Guid.NewGuid();

            _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

            _eventRepositoryMock
                .Setup(x => x.GetAllClubEventJoinerCountAsync(clubId))
                .ReturnsAsync(42);

            var query = new GetAllClubEventJoinerCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(42);
            result.Message.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ClubHasNoJoiners_ShouldReturnZero()
        {
            // Arrange
            var clubId = Guid.NewGuid();

            _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

            _eventRepositoryMock
                .Setup(x => x.GetAllClubEventJoinerCountAsync(clubId))
                .ReturnsAsync(0);

            var query = new GetAllClubEventJoinerCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(0);
            result.Message.Should().BeNull();
        }
    }
}
