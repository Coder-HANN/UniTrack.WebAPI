using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class GetEventQrQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly GetEventQrQueryHandler _handler;

    public GetEventQrQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventRepositoryMock = new Mock<IEventRepository>();

        _handler = new GetEventQrQueryHandler(
            _currentUserMock.Object,
            _eventRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_RoleIsNotClub_ShouldReturnFail()
    {
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);

        var result = await _handler.Handle(new GetEventQrQuery(), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ClubIdIsNull_ShouldReturnFail()
    {
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);

        var result = await _handler.Handle(new GetEventQrQuery(), CancellationToken.None);

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
            new GetEventQrQuery { EventId = Guid.NewGuid() },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnQrUrl()
    {
        var clubId = Guid.NewGuid();
        var qrUrl = "https://qr-code-url";

        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);

        _eventRepositoryMock
            .Setup(x => x.GetEventByIdAndClubIdAsync(It.IsAny<Guid>(), clubId))
            .ReturnsAsync(new Event { QrCodeUrl = qrUrl });

        var result = await _handler.Handle(
            new GetEventQrQuery { EventId = Guid.NewGuid() },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(qrUrl, result.Data);
    }
}
