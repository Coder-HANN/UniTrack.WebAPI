using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class GetEventQrQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationMock;
    private readonly GetEventQrQueryHandler _handler;

    public GetEventQrQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new GetEventQrQueryHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object,
            _localizationMock.Object);
    }

    [Fact]
    public async Task Handle_RoleIsNotClub_ShouldReturnFail()
    {
        // Arrange
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);
        _localizationMock
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Yetkiniz yok.");

        // Act
        var result = await _handler.Handle(new GetEventQrQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Yetkiniz yok.", result.Message);
    }

    [Fact]
    public async Task Handle_ClubIdIsNull_ShouldReturnFail()
    {
        // Arrange
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _localizationMock
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Yetkiniz yok.");

        // Act
        var result = await _handler.Handle(new GetEventQrQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Yetkiniz yok.", result.Message);
    }

    [Fact]
    public async Task Handle_EventNotFound_ShouldReturnFail()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);

        _eventRepositoryMock
            .Setup(x => x.GetEventByIdAndClubIdAsync(eventId, clubId))
            .ReturnsAsync((Event)null);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Etkinlik bulunamadı.");

        var query = new GetEventQrQuery { EventId = eventId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Etkinlik bulunamadı.", result.Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnQrUrl()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var qrUrl = "https://qr-code-url";

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);

        _eventRepositoryMock
            .Setup(x => x.GetEventByIdAndClubIdAsync(eventId, clubId))
            .ReturnsAsync(new Event { QrCodeUrl = qrUrl });

        var query = new GetEventQrQuery { EventId = eventId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(qrUrl, result.Data);
        Assert.Null(result.Message);
    }
}