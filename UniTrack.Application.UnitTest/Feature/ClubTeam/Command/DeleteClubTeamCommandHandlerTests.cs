using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.ClubTeam.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class DeleteClubTeamCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<IClubTeamRepository> _clubTeamRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private DeleteClubTeamCommandHandler CreateHandler()
        => new(_currentUserServices.Object, _clubTeamRepository.Object, _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Delete_ClubTeam_When_Valid()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var clubTeamId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);

        var clubTeam = new ClubTeam
        {
            Id = clubTeamId,
            ClubId = clubId,
            UserId = Guid.NewGuid(),
            Title = "Takım Lideri"
        };

        _clubTeamRepository.Setup(x => x.GetClubTeamId(clubTeamId))
            .ReturnsAsync(clubTeam);

        _localizationService.Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync((string key) => key);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new DeleteClubTeamCommand { ClubTeamId = clubTeamId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ValidationKeys.ClubTeamDeletedSuccess);

        _clubTeamRepository.Verify(x => x.DeleteAsync(clubTeam), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_NotAuthorized_When_ClubId_Is_Null()
    {
        // Arrange
        _currentUserServices.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new DeleteClubTeamCommand { ClubTeamId = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.NotAuthorized);
    }

    [Fact]
    public async Task Handle_Should_Return_ClubTeamNotFound_When_ClubTeam_Is_Null()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var clubTeamId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);

        _clubTeamRepository.Setup(x => x.GetClubTeamId(clubTeamId)).ReturnsAsync((ClubTeam)null);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new DeleteClubTeamCommand { ClubTeamId = clubTeamId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.ClubTeamNotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_ClubTeamNotFound_When_ClubTeam_ClubId_DoesNotMatch()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var wrongClubId = Guid.NewGuid();
        var clubTeamId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);

        var clubTeam = new ClubTeam
        {
            Id = clubTeamId,
            ClubId = wrongClubId, // Farklı club
            UserId = Guid.NewGuid(),
            Title = "Takım Lideri"
        };

        _clubTeamRepository.Setup(x => x.GetClubTeamId(clubTeamId)).ReturnsAsync(clubTeam);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string key) => key);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new DeleteClubTeamCommand { ClubTeamId = clubTeamId }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ValidationKeys.ClubTeamNotFound);
    }
}
