using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class GetEventSheetQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationMock;
    private readonly GetEventSheetQueryHandler _handler;

    public GetEventSheetQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new GetEventSheetQueryHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object,
            _localizationMock.Object); // Localization eklendi
    }

    [Fact]
    public async Task Handle_RoleIsNotClub_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);
        _localizationMock.Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Yetkiniz yok.");

        // Act
        var result = await _handler.Handle(new GetEventSheetQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Yetkiniz yok.");
        _eventRepositoryMock.Verify(x => x.GetEventByIdAndClubIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ClubIdIsNull_ShouldReturnFail()
    {
        // Arrange
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _localizationMock.Setup(x => x.Get(ValidationKeys.ClubNotFound))
            .ReturnsAsync("Kulüp bulunamadı.");

        // Act
        var result = await _handler.Handle(new GetEventSheetQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Kulüp bulunamadı.");
    }

    [Fact]
    public async Task Handle_EventNotFound_ShouldReturnFail()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _eventRepositoryMock.Setup(x => x.GetEventByIdAndClubIdAsync(eventId, clubId))
            .ReturnsAsync((Event)null);
        _localizationMock.Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Etkinlik bulunamadı.");

        // Act
        var result = await _handler.Handle(new GetEventSheetQuery { EventId = eventId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Etkinlik bulunamadı.");
    }

    [Fact]
    public async Task Handle_SheetsIdIsNull_ShouldReturnFail()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _eventRepositoryMock.Setup(x => x.GetEventByIdAndClubIdAsync(eventId, clubId))
            .ReturnsAsync(new Event { SheetsId = null });
        _localizationMock.Setup(x => x.Get(ValidationKeys.GoogleSheetsTableNotFound))
            .ReturnsAsync("Google Sheets tablosu bulunamadı.");

        // Act
        var result = await _handler.Handle(new GetEventSheetQuery { EventId = eventId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Google Sheets tablosu bulunamadı.");
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnFullSheetUrl()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var sheetId = "sheet123";
        var expectedUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/edit";

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _eventRepositoryMock.Setup(x => x.GetEventByIdAndClubIdAsync(eventId, clubId))
            .ReturnsAsync(new Event { SheetsId = sheetId });

        // Act
        var result = await _handler.Handle(new GetEventSheetQuery { EventId = eventId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedUrl);
    }
}