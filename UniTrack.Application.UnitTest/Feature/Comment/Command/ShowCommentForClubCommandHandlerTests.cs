using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Application.Feature.Comment.Command;
using Xunit;

public class ShowCommentForClubCommandHandlerTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private ShowCommentForClubCommandHandler CreateHandler()
        => new ShowCommentForClubCommandHandler(
            _commentRepository.Object,
            _currentUserServices.Object,
            _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Return_CalculatedAverage_WhenUserIsAuthorized()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        double expectedAverage = 4.3;
        int expectedCount = 10;

        // Kullanıcının bir kulübe ait olduğunu simüle ediyoruz
        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);

        // Repository'den tuple (average, count) dönüyoruz
        _commentRepository
            .Setup(x => x.GetClubAverageRatingAsync(clubId))
            .ReturnsAsync((expectedAverage, expectedCount));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ShowCommentForClubCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Point.Should().Be(expectedAverage);
        result.Data.CommentCount.Should().Be(expectedCount);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_ClubId_IsNull()
    {
        // Arrange
        _currentUserServices.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync("Not Authorized");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ShowCommentForClubCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Not Authorized");
        _commentRepository.Verify(x => x.GetClubAverageRatingAsync(It.IsAny<Guid>()), Times.Never);
    }
}