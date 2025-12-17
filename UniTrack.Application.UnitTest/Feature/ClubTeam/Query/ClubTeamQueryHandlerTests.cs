using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.ClubTeam.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class ClubTeamQueryHandlerTests
{
    private readonly Mock<IClubTeamRepository> _clubTeamRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private ClubTeamQueryHandler CreateHandler()
        => new(_clubTeamRepository.Object, _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Return_ClubTeams_Successfully()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        var clubTeams = new List<ClubTeam>
        {
            new ClubTeam
            {
                ClubId = clubId,
                UserDetail = new UserDetail
                {
                    Name = "Ali",
                    Surname = "Yılmaz"
                },
                Title = "Takım Lideri"
            },
            new ClubTeam
            {
                ClubId = clubId,
                UserDetail = new UserDetail
                {
                    Name = "Ayşe",
                    Surname = "Demir"
                },
                Title = "Üye"
            }
        };

        _clubTeamRepository
            .Setup(x => x.GetClubTeamsByClubIdAsync(clubId))
            .ReturnsAsync(clubTeams);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ClubTeamQuery(clubId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data[0].PersonName.Should().Be("Ali");
        result.Data[0].PersonSurname.Should().Be("Yılmaz");
        result.Data[0].Title.Should().Be("Takım Lideri");
        result.Data[1].PersonName.Should().Be("Ayşe");
        result.Data[1].PersonSurname.Should().Be("Demir");
        result.Data[1].Title.Should().Be("Üye");
    }

    [Fact]
    public async Task Handle_Should_Return_ClubTeamNotFound_When_No_ClubTeams()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _clubTeamRepository
            .Setup(x => x.GetClubTeamsByClubIdAsync(clubId))
            .ReturnsAsync((List<ClubTeam>)null);

        _localizationService
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync((string key) => key);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ClubTeamQuery(clubId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be(ValidationKeys.ClubTeamNotFound);
    }
}
