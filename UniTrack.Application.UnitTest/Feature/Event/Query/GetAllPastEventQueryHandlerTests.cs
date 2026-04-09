using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetAllPastEventQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<IBaseEntityRepository<Event>> _pagingRepoMock;
    private readonly Mock<ILocalizationService> _localizationMock;
    private readonly GetAllPastEventQueryHandler _handler;

    public GetAllPastEventQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _pagingRepoMock = new Mock<IBaseEntityRepository<Event>>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new GetAllPastEventQueryHandler(
            _currentUserMock.Object,
            _pagingRepoMock.Object,
            _eventRepositoryMock.Object,
            _localizationMock.Object // Localization eklendi
        );
    }

    [Fact]
    public async Task Handle_NoPastEvents_ShouldReturnSuccessWithEmptyPagedResult()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _eventRepositoryMock.Setup(x => x.GetPastEventsAsync()).ReturnsAsync(new List<Event>());

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Geçmiş etkinlik bulunamadı.");

        var pagedResult = new Mock<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>();

        _pagingRepoMock
            .Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllPastEventQueryResponseDTO>>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult.Object);

        // Act
        var result = await _handler.Handle(new GetAllPastEventQuery(1, 10), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Geçmiş etkinlik bulunamadı.", result.Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnPagedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(userId);

        var events = new List<Event>
        {
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Geçmiş Etkinlik",
                StartDate = DateTime.UtcNow.AddDays(-10),
                Quota = 100,
                Club = new Club { Name = "Test Club", UserClubs = new List<UserClub>() },
                EventUsers = new List<EventUser>(),
                Images = new List<EventImage>()
            }
        };

        _eventRepositoryMock.Setup(x => x.GetPastEventsAsync()).ReturnsAsync(events);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.GetEventSuccesses))
            .ReturnsAsync("Başarılı");

        var pagedResult = new Mock<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>();

        _pagingRepoMock
            .Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllPastEventQueryResponseDTO>>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult.Object);

        // Act
        var result = await _handler.Handle(new GetAllPastEventQuery(1, 10), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Başarılı", result.Message);
    }
}