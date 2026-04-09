using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetClubFavoriteThreeEventsQueryHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<ICommentRepository> _commentRepositoryMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();

    private GetClubFavoriteThreeEventsQueryHandler CreateHandler()
        => new(
            _eventRepositoryMock.Object,
            _commentRepositoryMock.Object,
            _localizationMock.Object
        );

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithEmptyData_WhenNoEventsFound()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        _eventRepositoryMock
            .Setup(x => x.GetTopThreeFavoriteEventsByClubIdAsync(clubId))
            .ReturnsAsync(new List<Event>()); // Boş liste dönüyor

        _localizationMock.Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Etkinlik bulunamadı");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetClubFavoriteThreeEventsQuery { ClubId = clubId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue(); // Handler kodunda boş listede IsSuccess = true dönüyor
        result.Data.Should().BeNull();
        result.Message.Should().Be("Etkinlik bulunamadı");
    }

    [Fact]
    public async Task Handle_ShouldReturnFavoriteEvents_WithCorrectMapping()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var favoriteEvents = new List<Event>
        {
            new Event
            {
                Id = eventId,
                Title = "Yazılım Kampı",
                Joiner = 50,
                Quota = 100,
                StartDate = DateTime.Now.AddDays(1),
                Location = "İstanbul",
                Images = new List<EventImage>
                {
                    new EventImage { ImageUrl = "cover.jpg", IsCover = true, Order = 1 }
                }
            }
        };

        // Tuple (AverageRating, PointsCount)
        var ratingsDict = new Dictionary<Guid, (float, int)>
        {
            { eventId, (4.8f, 25) }
        };

        _eventRepositoryMock
            .Setup(x => x.GetTopThreeFavoriteEventsByClubIdAsync(clubId))
            .ReturnsAsync(favoriteEvents);

        _commentRepositoryMock
            .Setup(x => x.GetEventsRatingsSummaryAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(ratingsDict);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetClubFavoriteThreeEventsQuery { ClubId = clubId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(1);

        var dto = result.Data.First();
        dto.EventName.Should().Be("Yazılım Kampı");
        dto.Points.Should().Be(4.8f);
        dto.PointsCount.Should().Be(25);
        dto.CoverImageUrl.Should().Be("cover.jpg");
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultRatings_WhenNoRatingsExistInDictionary()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var favoriteEvents = new List<Event> { new Event { Id = eventId, Title = "No Rating Event" } };

        _eventRepositoryMock.Setup(x => x.GetTopThreeFavoriteEventsByClubIdAsync(clubId)).ReturnsAsync(favoriteEvents);

        // Dictionary'de bu event id yok
        _commentRepositoryMock.Setup(x => x.GetEventsRatingsSummaryAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new Dictionary<Guid, (float, int)>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetClubFavoriteThreeEventsQuery { ClubId = clubId }, CancellationToken.None);

        // Assert
        var dto = result.Data.First();
        dto.Points.Should().Be(0f); // Handler: GetValueOrDefault(e.Id, (0f, 0))
        dto.PointsCount.Should().Be(0);
    }
}