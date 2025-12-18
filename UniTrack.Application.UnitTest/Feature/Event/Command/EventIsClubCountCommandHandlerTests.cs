using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.DTOs.Event;

public class EventIsClubCountCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly EventIsClubCountCommandHandler _handler;

    public EventIsClubCountCommandHandlerTests()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();

        _handler = new EventIsClubCountCommandHandler(
            _eventRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Return_Event_Count_For_Club()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _eventRepositoryMock
            .Setup(x => x.GetClubEventCountAsync(clubId))
            .ReturnsAsync(5);

        var command = new EventIsClubCountCommand
        {
            ClubId = clubId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.EventCount.Should().Be(5);

        _eventRepositoryMock.Verify(
            x => x.GetClubEventCountAsync(clubId),
            Times.Once);
    }
}
