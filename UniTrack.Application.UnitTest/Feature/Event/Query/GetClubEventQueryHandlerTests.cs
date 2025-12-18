using Moq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetClubEventQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly GetClubEventQueryHandler _handler;

    public GetClubEventQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();

        _handler = new GetClubEventQueryHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNoEventsFound()
    {
        var clubId = Guid.NewGuid();

        _eventRepositoryMock
            .Setup(x => x.GetAllClubEventAsync(It.IsAny<Expression<Func<Event, bool>>>()))
            .ReturnsAsync(new List<Event>());

        var result = await _handler.Handle(
            new GetClubEventQuery(clubId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnEventsSuccessfully()
    {
        var clubId = Guid.NewGuid();

        var events = new List<Event>
        {
            new Event { Title = "Event 1", ClubId = clubId },
            new Event { Title = "Event 2", ClubId = clubId }
        };

        _eventRepositoryMock
            .Setup(x => x.GetAllClubEventAsync(It.IsAny<Expression<Func<Event, bool>>>()))
            .ReturnsAsync(events);

        var result = await _handler.Handle(
            new GetClubEventQuery(clubId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
    }
}
