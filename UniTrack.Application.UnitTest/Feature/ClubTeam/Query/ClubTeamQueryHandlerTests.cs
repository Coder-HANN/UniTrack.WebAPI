using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.ClubTeam.Query;
using UniTrack.Domain.Entities;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.DTOs.ClubTeam;

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
        var mockImageUrl = "https://unitrack.com/ali.jpg";

        // Handler ct.User.UserDetail üzerinden veriye eriştiği için 
        // mock datayı bu hiyerarşiye uygun kurmalıyız.
        var clubTeams = new List<ClubTeam>
        {
            new ClubTeam
            {
                Id = Guid.NewGuid(),
                ClubId = clubId,
                Title = "Takım Lideri",
                User = new User
                {
                    UserDetail = new UserDetail
                    {
                        Name = "Ali",
                        Surname = "Yılmaz",
                        ProfileImageUrl = mockImageUrl
                    }
                }
            },
            new ClubTeam
            {
                Id = Guid.NewGuid(),
                ClubId = clubId,
                Title = "Üye",
                User = new User
                {
                    UserDetail = new UserDetail
                    {
                        Name = "Ayşe",
                        Surname = "Demir",
                        ProfileImageUrl = null
                    }
                }
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

        // İlk eleman kontrolü
        result.Data[0].PersonName.Should().Be("Ali");
        result.Data[0].Title.Should().Be("Takım Lideri");
        result.Data[0].ProfileImageUrl.Should().Be(mockImageUrl);

        // İkinci eleman kontrolü
        result.Data[1].PersonName.Should().Be("Ayşe");
        result.Data[1].Title.Should().Be("Üye");
    }

    [Fact]
    public async Task Handle_Should_Return_ClubTeamNotFound_When_No_ClubTeams()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        string expectedError = "Takım bulunamadı";

        _clubTeamRepository
            .Setup(x => x.GetClubTeamsByClubIdAsync(clubId))
            .ReturnsAsync((List<ClubTeam>)null);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.ClubTeamNotFound))
            .ReturnsAsync(expectedError);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new ClubTeamQuery(clubId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(expectedError);
        result.Data.Should().BeNull();
    }
}