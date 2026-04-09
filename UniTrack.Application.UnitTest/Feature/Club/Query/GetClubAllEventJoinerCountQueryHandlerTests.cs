using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Query;
using Xunit;

public class GetClubAllEventJoinerCountQueryHandlerTests
{
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock;
    private readonly Mock<ICurrentUserServices> _currentUserServicesMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly GetClubAllEventJoinerCountQueryHandler _handler;

    public GetClubAllEventJoinerCountQueryHandlerTests()
    {
        _eventUserRepositoryMock = new Mock<IEventUserRepository>();
        _currentUserServicesMock = new Mock<ICurrentUserServices>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _handler = new GetClubAllEventJoinerCountQueryHandler(
            _eventUserRepositoryMock.Object,
            _currentUserServicesMock.Object,
            _localizationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenNotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        _currentUserServicesMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _localizationServiceMock.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync("Unauthorized");

        var query = new GetClubAllEventJoinerCountQuery(); // Parametresiz oluşturuldu

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_WhenJoinerCountIsZero_ReturnsFail()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        _currentUserServicesMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _eventUserRepositoryMock.Setup(x => x.GetTotalJoinerCountByClubIdAsync(clubId)).ReturnsAsync(0);

        var query = new GetClubAllEventJoinerCountQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenSuccess_ReturnsCount()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        _currentUserServicesMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _eventUserRepositoryMock.Setup(x => x.GetTotalJoinerCountByClubIdAsync(clubId)).ReturnsAsync(15);

        var query = new GetClubAllEventJoinerCountQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(15);
    }
}