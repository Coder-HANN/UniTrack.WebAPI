using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Query;
using Xunit;

public class GetFollowClubCountQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IUserClubRepository> _userClubRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    public GetFollowClubCountQueryHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _userClubRepository = new Mock<IUserClubRepository>();
        _localizationService = new Mock<ILocalizationService>();
    }

    private GetFollowClubCountQueryHandler CreateHandler()
        => new GetFollowClubCountQueryHandler(
            _currentUserServices.Object,
            _userClubRepository.Object,
            _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Return_NotAuthorized_When_User_Not_Logged_In()
    {
        // Arrange
        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns((Guid?)null);
        _localizationService
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync("Not authorized");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetFollowClubCountQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0);
        result.Message.Should().Be("Not authorized");
        _userClubRepository.Verify(
            x => x.GetFollowedClubCountAsync(It.IsAny<Guid>()),
            Times.Never);
    }
}