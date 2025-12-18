using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class GetEventSheetQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly GetEventSheetQueryHandler _handler;

    public GetEventSheetQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();

        _handler = new GetEventSheetQueryHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_RoleIsNotClub_ShouldReturnUnauthorized()
    {
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);

        var result = await _handler.Handle(new GetEventSheetQuery(), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ClubIdIsNull_ShouldReturnFail()
    {
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);

        var result = await _handler.Handle(new GetEventSheetQuery(), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_EventNotFound_ShouldReturnFail()
    {
        var clubId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);

        _eventRepositoryMock
            .Setup(x => x.GetEventByIdAndClubIdAsync(It.IsAny<Guid>(), clubId))
            .ReturnsAsync((Event)null);

        var result = await _handler.Handle(
            new GetEventSheetQuery { EventId = Guid.NewGuid() },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_SheetsIdIsNull_ShouldReturnFail()
    {
        var clubId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);

        _eventRepositoryMock
            .Setup(x => x.GetEventByIdAndClubIdAsync(It.IsAny<Guid>(), clubId))
            .ReturnsAsync(new Event { SheetsId = null });

        var result = await _handler.Handle(
            new GetEventSheetQuery { EventId = Guid.NewGuid() },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnSheetUrl()
    {
        var clubId = Guid.NewGuid();
        var sheetId = "sheet123";

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);

        _eventRepositoryMock
            .Setup(x => x.GetEventByIdAndClubIdAsync(It.IsAny<Guid>(), clubId))
            .ReturnsAsync(new Event { SheetsId = sheetId });

        var result = await _handler.Handle(
            new GetEventSheetQuery { EventId = Guid.NewGuid() },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(sheetId, result.Data);
    }
}
