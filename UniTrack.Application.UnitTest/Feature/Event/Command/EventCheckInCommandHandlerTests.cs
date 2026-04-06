using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Feature.Event.Command.EventCheckInCommandHandler;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Domain.Entities;
using UniTrack.Application.Common.Constants;

public class EventCheckInCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IParticipantSheetRepository> _sheetRepositoryMock = new();
    private readonly Mock<ICurrentUserServices> _currentUserServicesMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();

    private readonly EventCheckInCommandHandler _handler;

    public EventCheckInCommandHandlerTests()
    {
        _handler = new EventCheckInCommandHandler(
            _eventRepositoryMock.Object,
            _userRepositoryMock.Object,
            _sheetRepositoryMock.Object,
            _currentUserServicesMock.Object,
            _eventUserRepositoryMock.Object,
            _localizationMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidToken_ShouldUpdateDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventCheckInToken = Guid.NewGuid(); // QR'dan gelen Token
        var actualEventId = Guid.NewGuid();     // DB'deki gerçek PK

        _currentUserServicesMock.Setup(x => x.CurrentUser()).Returns(userId);

        // 1. ADIM: Handler artık GetCheckinIdAsync (Token ile) çağırmalı
        _eventRepositoryMock
            .Setup(x => x.GetCheckinIdAsync(eventCheckInToken))
            .ReturnsAsync(new Event
            {
                Id = actualEventId,
                CheckInToken = eventCheckInToken,
                EndDate = DateTimeOffset.UtcNow.AddDays(1),
                SheetsId = "sheet-123"
            });

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Email = "test@test.com" });

        // 2. ADIM: EventUser mutlaka DOLU dönmeli (Kullanıcı etkinliğe kayıtlı olmalı)
        var eventUser = new EventUser
        {
            UserId = userId,
            EventId = actualEventId,
            IsCheckedIn = false
        };

        _eventUserRepositoryMock
            .Setup(x => x.GetEventUserCheckInAsync(userId, eventCheckInToken))
            .ReturnsAsync(eventUser);

        _localizationMock
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync("Success");

        var command = new EventCheckInCommand { EventCheckInId = eventCheckInToken };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // 3. ADIM: VERİTABANI GÜNCELLEME KONTROLÜ
        // UpdateAsync'in tam olarak bu nesneyle çağrıldığını doğrula
        _eventUserRepositoryMock.Verify(x =>
            x.UpdateAsync(It.Is<EventUser>(eu =>
                eu.IsCheckedIn == true &&
                eu.UserId == userId)),
            Times.Once);

        // Google Sheets kontrolü
        _sheetRepositoryMock.Verify(x =>
            x.MarkUserAsCheckedInAsync("sheet-123", "test@test.com"),
            Times.Once);
    }
}
