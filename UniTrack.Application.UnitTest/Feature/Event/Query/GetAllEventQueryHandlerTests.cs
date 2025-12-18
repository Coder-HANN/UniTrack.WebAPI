using System;
using System.Collections.Generic;
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
using System.Linq.Expressions;

public class GetAllEventQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IBaseEntityRepository<Event>> _baseRepositoryMock = new();

    private GetAllEventQueryHandler CreateHandler()
        => new(
            _currentUserMock.Object,
            _eventRepositoryMock.Object,
            _baseRepositoryMock.Object);

    [Fact]
    public async Task Handle_NoEvents_ReturnsFail()
    {
        _eventRepositoryMock
            .Setup(x => x.GetAllEventAsync(It.IsAny<Expression<Func<Event, bool>>>()))
            .ReturnsAsync(new List<Event>());

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllEventQuery(1, 10), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_EventsExist_ReturnsPagedResult()
    {
        var events = new List<Event>
        {
            new Event
            {
                Title = "Event 1",
                IsDeleted = false,
                StartDate = DateTime.UtcNow
            }
        };

        _eventRepositoryMock
            .Setup(x => x.GetAllEventAsync(It.IsAny<Expression<Func<Event, bool>>>()))
            .ReturnsAsync(events);

        var pagingResultMock = new Mock<IPagingExecutionResult<GetAllEventQueryResponseDTO>>();

        _baseRepositoryMock.Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllEventQueryResponseDTO>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Func<IQueryable<GetAllEventQueryResponseDTO>, IOrderedQueryable<GetAllEventQueryResponseDTO>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResultMock.Object);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllEventQuery(1, 10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
