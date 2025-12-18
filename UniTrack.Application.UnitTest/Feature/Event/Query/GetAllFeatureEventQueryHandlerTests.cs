using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Entities;

public class GetAllFeatureEventQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IBaseEntityRepository<Event>> _pagingRepositoryMock = new();

    private GetAllFeatureEventQueryHandler CreateHandler()
        => new(
            _currentUserMock.Object,
            _pagingRepositoryMock.Object,
            _eventRepositoryMock.Object);

    [Fact]
    public async Task Handle_UserNotLoggedIn_ReturnsUnauthorized()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllFeatureEventQuery(1, 10), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NoFeaturedEvents_ReturnsFail()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _eventRepositoryMock.Setup(x => x.GetFeatureEventsAsync())
            .ReturnsAsync(new List<Event>());

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllFeatureEventQuery(1, 10), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPagedFeaturedEvents()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

        var events = new List<Event>
        {
            new Event
            {
                Title = "Test Event",
                StartDate = DateTime.UtcNow,
                Quota = 100
            }
        };

        _eventRepositoryMock.Setup(x => x.GetFeatureEventsAsync())
            .ReturnsAsync(events);

        var pagingResultMock =
     new Mock<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>();

        _pagingRepositoryMock
            .Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllFeatureEventQueryResponseDTO>>(),
                It.IsAny<int?>(),   // 🔴 MUTLAKA int?
                It.IsAny<int?>(),   // 🔴 MUTLAKA int?
                It.IsAny<Func<IQueryable<GetAllFeatureEventQueryResponseDTO>,
                               IOrderedQueryable<GetAllFeatureEventQueryResponseDTO>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResultMock.Object);


        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllFeatureEventQuery(1, 10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
