using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Application.Feature.Comment.Command;
using Xunit;

public class ShowCommentForClubCommandHandlerTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private ShowCommentForClubCommandHandler CreateHandler()
        => new ShowCommentForClubCommandHandler(_commentRepository.Object);

    [Fact]
    public async Task Handle_Should_Return_CalculatedAverage_WhenAverageExists()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        double average = 4.3;

        _commentRepository
            .Setup(x => x.GetClubAverageRatingAsync(clubId))
            .ReturnsAsync(average);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ShowCommentForClubCommand { ClubId = clubId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Point.Should().BeApproximately(4.3, 0.01);
    }

    [Fact]
    public async Task Handle_Should_Return_Zero_WhenAverageIsNull()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        double average = 0;
        _commentRepository
            .Setup(x => x.GetClubAverageRatingAsync(clubId))
            .ReturnsAsync(average);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ShowCommentForClubCommand { ClubId = clubId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Point.Should().Be(0);
    }
}
