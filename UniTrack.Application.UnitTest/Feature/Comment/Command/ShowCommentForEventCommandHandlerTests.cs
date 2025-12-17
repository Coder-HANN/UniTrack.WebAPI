using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Feature.Comment.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;

public class ShowCommentForEventCommandHandlerTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();

    private ShowCommentForEventCommandHandler CreateHandler()
        => new ShowCommentForEventCommandHandler(_commentRepository.Object);

    [Fact]
    public async Task Handle_Should_Return_Average_WhenValueExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        double average = 4.5;

        _commentRepository
            .Setup(x => x.GetEventAverageRatingAsync(eventId))
            .ReturnsAsync(average);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ShowCommentForEventCommand { EventId = eventId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Point.Should().Be(average);
    }

    [Fact]
    public async Task Handle_Should_Return_Zero_WhenAverageIsNull()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        double average = 0;

        _commentRepository
            .Setup(x => x.GetEventAverageRatingAsync(eventId))
            .ReturnsAsync(average);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ShowCommentForEventCommand { EventId = eventId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Point.Should().Be(0);
    }
}
