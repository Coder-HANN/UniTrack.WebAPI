using Xunit;
using Moq;
using FluentAssertions;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UniTrack.Application.Tests.Feature.Event.Query
{
    public class GetAllClubEventJoinerCountQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly Mock<ILocalizationService> _localizationServiceMock = new();
        private readonly GetAllClubEventJoinerCountQueryHandler _handler;

        public GetAllClubEventJoinerCountQueryHandlerTests()
        {
            _handler = new GetAllClubEventJoinerCountQueryHandler(
                _currentUserMock.Object,
                _eventRepositoryMock.Object,
                _localizationServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ClubHasJoiners_ShouldReturnJoinerCount()
        {
            // Arrange
            var clubId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

            // KRİTİK DÜZELTME: ReturnsAsync içerisine 42 yerine açıkça 42L (long) verdik.
            // Ayrıca parametre uyuşmazlığını tamamen sıfırlamak için It.IsAny<Guid?>() kullandık.
            _eventRepositoryMock
                .Setup(x => x.GetClubEventCheckedInCountAsync(It.IsAny<Guid?>()))
                .ReturnsAsync(42L);

            var query = new GetAllClubEventJoinerCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(42L); // Assert ederken de long (42L) doğruluyoruz
            result.Message.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ClubHasNoJoiners_ShouldReturnZero()
        {
            // Arrange
            var clubId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
            _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

            // KRİTİK DÜZELTME: ReturnsAsync içerisine 0 yerine açıkça 0L (long) verdik.
            _eventRepositoryMock
                .Setup(x => x.GetClubEventCheckedInCountAsync(It.IsAny<Guid?>()))
                .ReturnsAsync(0L);

            var query = new GetAllClubEventJoinerCountQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(0L); // Assert ederken de long (0L) doğruluyoruz
            result.Message.Should().BeNull();
        }
    }
}