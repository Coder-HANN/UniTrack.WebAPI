using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetClubFavoriteThreeEventsQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly GetClubFavoriteThreeEventsQueryHandler _handler;

    public GetClubFavoriteThreeEventsQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _commentRepositoryMock = new Mock<ICommentRepository>();

        _handler = new GetClubFavoriteThreeEventsQueryHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object,
            _commentRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUnauthorized()
    {
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _currentUserMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);

        var result = await _handler.Handle(
            new GetClubFavoriteThreeEventsQuery { ClubId = Guid.NewGuid() },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNoFavoriteEvents()
    {
        var clubId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(userId);

        _eventRepositoryMock
            .Setup(x => x.GetTopThreeFavoriteEventsByClubIdAsync(clubId))
            .ReturnsAsync(new List<Event>());

        var result = await _handler.Handle(
            new GetClubFavoriteThreeEventsQuery { ClubId = clubId },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnFavoriteEventsSuccessfully()
    {
        var clubId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Title = "Event 1", Joiner = 10, Quota = 100 },
            new Event { Id = Guid.NewGuid(), Title = "Event 2", Joiner = 8, Quota = 80 },
            new Event { Id = Guid.NewGuid(), Title = "Event 3", Joiner = 6, Quota = 60 }
        };

        var ratings = new Dictionary<Guid, (float, int)>
        {
            { events[0].Id, (4.5f, 10) },
            { events[1].Id, (4.2f, 8) },
            { events[2].Id, (3.9f, 6) }
        };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(userId);

        _eventRepositoryMock
            .Setup(x => x.GetTopThreeFavoriteEventsByClubIdAsync(clubId))
            .ReturnsAsync(events);

        _commentRepositoryMock
            .Setup(x => x.GetEventsRatingsSummaryAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(ratings);

        var result = await _handler.Handle(
            new GetClubFavoriteThreeEventsQuery { ClubId = clubId },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Data.Count);
    }
}
