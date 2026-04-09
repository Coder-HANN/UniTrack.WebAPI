using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetClubEventQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();
    private readonly Mock<IBaseEntityRepository<Event>> _baseEntityRepoMock = new();
    private readonly GetClubEventQueryHandler _handler;

    public GetClubEventQueryHandlerTests()
    {
        _handler = new GetClubEventQueryHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object,
            _localizationMock.Object,
            _baseEntityRepoMock.Object
        );
    }
    [Fact]
    public async Task Handle_WhenNoEventsFound_ShouldReturnEmptyPagedResultWithNotFoundMessage()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        // Constructor artık (clubId, page, pageSize) bekliyor:
        var query = new GetClubEventQuery(clubId, 1, 10);

        _eventRepositoryMock
            .Setup(x => x.GetAllClubEventAsync(It.IsAny<Expression<Func<Event, bool>>>()))
            .ReturnsAsync(new List<Event>());

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Event not found");

        var pagedResult = new Mock<IPagingExecutionResult<GetClubEventQueryResponseDTO>>();
        _baseEntityRepoMock
            .Setup(x => x.GetPagedResult(It.IsAny<IEnumerable<GetClubEventQueryResponseDTO>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Func<IQueryable<GetClubEventQueryResponseDTO>, IOrderedQueryable<GetClubEventQueryResponseDTO>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Event not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnEventsSuccessfully_WithPagedData()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        // Constructor hatası burada da düzeltildi:
        var query = new GetClubEventQuery(clubId, 1, 10);

        var events = new List<Event>
    {
        new Event {
            Id = Guid.NewGuid(),
            Title = "Event 1",
            ClubId = clubId,
            EventUsers = new List<EventUser>(),
            Images = new List<EventImage>()
        }
    };

        _eventRepositoryMock
            .Setup(x => x.GetAllClubEventAsync(It.IsAny<Expression<Func<Event, bool>>>()))
            .ReturnsAsync(events);

        var pagedResult = new Mock<IPagingExecutionResult<GetClubEventQueryResponseDTO>>();
        _baseEntityRepoMock
            .Setup(x => x.GetPagedResult(It.IsAny<IEnumerable<GetClubEventQueryResponseDTO>>(),
                query.PageSize, query.Page, // Parametrelerin uyumlu olduğundan emin oluyoruz
                It.IsAny<Func<IQueryable<GetClubEventQueryResponseDTO>, IOrderedQueryable<GetClubEventQueryResponseDTO>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(pagedResult.Object);
    }
}