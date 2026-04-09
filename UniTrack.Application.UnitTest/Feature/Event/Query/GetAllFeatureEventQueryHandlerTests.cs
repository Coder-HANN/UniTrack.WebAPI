using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Entities;

public class GetAllFeatureEventQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IBaseEntityRepository<Event>> _pagingRepositoryMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();
    private readonly Mock<IEventImageRepository> _eventImageRepositoryMock = new();
    private readonly Mock<IUserClubRepository> _userClubRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    private GetAllFeatureEventQueryHandler CreateHandler()
        => new(
            _currentUserMock.Object,
            _pagingRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _localizationMock.Object,
            _eventImageRepositoryMock.Object,
            _userClubRepositoryMock.Object,
            _eventUserRepositoryMock.Object);

    [Fact]
    public async Task Handle_NoEventsFound_ReturnsSuccessWithEmptyPagedResult()
    {
        // Arrange
        _eventRepositoryMock.Setup(x => x.GetFeatureEventsAsync())
            .ReturnsAsync(new List<Event>());

        _localizationMock.Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Etkinlik bulunamadı.");

        var pagedResult = new Mock<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>();
        _pagingRepositoryMock.Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllFeatureEventQueryResponseDTO>>(),
                It.IsAny<int?>(), It.IsAny<int?>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult.Object);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllFeatureEventQuery(1, 10), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Etkinlik bulunamadı.");
    }

    [Fact]
    public async Task Handle_ValidRequest_WithFollowedClubs_ReturnsOrderedEvents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clubId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(userId);

        var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Title = "Event 1", ClubId = clubId, EventUsers = new List<EventUser>(), StartDate = DateTime.Now },
            new Event { Id = Guid.NewGuid(), Title = "Event 2", ClubId = Guid.NewGuid(), EventUsers = new List<EventUser>(), StartDate = DateTime.Now.AddDays(1) }
        };

        _eventRepositoryMock.Setup(x => x.GetFeatureEventsAsync()).ReturnsAsync(events);

        // Takip edilen kulüpleri mockla
        _userClubRepositoryMock.Setup(x => x.GetFollowedClubsByUserIdAsync(userId))
            .ReturnsAsync(new List<UserClub> { new UserClub { ClubId = clubId } });

        // Resimleri mockla
        _eventImageRepositoryMock.Setup(x => x.GetByEventIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<EventImage>());

        _localizationMock.Setup(x => x.Get(ValidationKeys.GetEventSuccesses))
            .ReturnsAsync("Başarılı");

        var pagedResult = new Mock<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>();
        _pagingRepositoryMock.Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllFeatureEventQueryResponseDTO>>(),
                It.IsAny<int?>(), It.IsAny<int?>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult.Object);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetAllFeatureEventQuery(1, 10), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(pagedResult.Object);
        _userClubRepositoryMock.Verify(x => x.GetFollowedClubsByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotLoggedIn_DoesNotCallUserClubRepository()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _eventRepositoryMock.Setup(x => x.GetFeatureEventsAsync())
            .ReturnsAsync(new List<Event> { new Event { Id = Guid.NewGuid(), EventUsers = new List<EventUser>() } });

        _eventImageRepositoryMock.Setup(x => x.GetByEventIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<EventImage>());

        var pagedResult = new Mock<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>();
        _pagingRepositoryMock.Setup(x => x.GetPagedResult(It.IsAny<IEnumerable<GetAllFeatureEventQueryResponseDTO>>(),
            It.IsAny<int?>(), It.IsAny<int?>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult.Object);

        var handler = CreateHandler();

        // Act
        await handler.Handle(new GetAllFeatureEventQuery(1, 10), CancellationToken.None);

        // Assert
        _userClubRepositoryMock.Verify(x => x.GetFollowedClubsByUserIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}