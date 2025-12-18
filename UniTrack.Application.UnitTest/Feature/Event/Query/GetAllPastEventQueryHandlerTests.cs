using Moq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetAllPastEventQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<IBaseEntityRepository<Event>> _pagingRepoMock;
    private readonly GetAllPastEventQueryHandler _handler;

    public GetAllPastEventQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _pagingRepoMock = new Mock<IBaseEntityRepository<Event>>();

        _handler = new GetAllPastEventQueryHandler(
            _currentUserMock.Object,
            _pagingRepoMock.Object,
            _eventRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_UserNull_ShouldReturnUnauthorized()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);

        var result = await _handler.Handle(new GetAllPastEventQuery(1, 10), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NoPastEvents_ShouldReturnFail()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

        _eventRepositoryMock
            .Setup(x => x.GetPastEventsAsync())
            .ReturnsAsync(new List<Event>());

        var result = await _handler.Handle(new GetAllPastEventQuery(1, 10), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnPagedResult()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

        var events = new List<Event>
        {
            new Event
            {
                Title = "Geçmiş Etkinlik",
                StartDate = DateTime.UtcNow.AddDays(-10)
            }
        };

        _eventRepositoryMock
            .Setup(x => x.GetPastEventsAsync())
            .ReturnsAsync(events);

        _pagingRepoMock
     .Setup(x => x.GetPagedResult<Event>(
         It.IsAny<IEnumerable<Event>>(),
         It.IsAny<int?>(),
         It.IsAny<int?>(),
         It.IsAny<Func<IQueryable<Event>, IOrderedQueryable<Event>>>(),
         It.IsAny<CancellationToken>()))
     .ReturnsAsync(Mock.Of<IPagingExecutionResult<Event>>());

        var result = await _handler.Handle(new GetAllPastEventQuery(1, 10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
