using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.ClubTeam.Command;
using UniTrack.Domain.Enums;
using UniTrack.Domain.Entities;
using Xunit;

public class CreateClubTeamCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<IClubTeamRepository> _clubTeamRepository = new();
    private readonly Mock<IUserClubRepository> _userClubRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private CreateClubTeamCommandHandler CreateHandler()
        => new(
            _currentUserServices.Object,
            _clubTeamRepository.Object,
            _userClubRepository.Object,
            _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Fail_When_User_Not_Authorized()
    {
        // Arrange
        var command = new CreateClubTeamCommand { ClubId = Guid.NewGuid(), UserId = Guid.NewGuid(), Title = "Team A" };
        _currentUserServices.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _localizationService.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync("Unauthorized");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_Should_Fail_When_User_Not_Following_Club()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var userDetailId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);

        // ✅ DÜZELTİLDİ: Takip etmeme senaryosu için null dönmesini sağlıyoruz.
        _userClubRepository
            .Setup(x => x.GetClubFollowersByUserIdAsync(clubId, userDetailId))
            .ReturnsAsync((UserClub)null);

        _localizationService.Setup(x => x.Get(ValidationKeys.UserMustFollowClub))
            .ReturnsAsync("User must follow the club");

        var command = new CreateClubTeamCommand { ClubId = clubId, UserId = userDetailId, Title = "Team A" };
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("User must follow the club");
    }

    [Fact]
    public async Task Handle_Should_Create_ClubTeam_Successfully()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var userDetailId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);

        // ✅ Başarılı senaryo için takipçi nesnesi dönüyoruz.
        _userClubRepository
            .Setup(x => x.GetClubFollowersByUserIdAsync(clubId, userDetailId))
            .ReturnsAsync(new UserClub { ClubId = clubId, UserId = userDetailId });

        _localizationService.Setup(x => x.Get(ValidationKeys.ClubTeamCreatedSuccess))
            .ReturnsAsync("Club team created successfully");

        var command = new CreateClubTeamCommand { ClubId = clubId, UserId = userDetailId, Title = "Team A" };
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Club team created successfully");

        _clubTeamRepository.Verify(x => x.AddAsync(It.Is<ClubTeam>(
            ct => ct.ClubId == clubId && ct.UserId == userDetailId && ct.Title == "Team A"
        )), Times.Once);
    }
}