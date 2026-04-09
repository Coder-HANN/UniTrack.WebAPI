using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class GetClubIsFollowerQueryHandlerTests
{
    private readonly Mock<IUserClubRepository> _userClubRepository = new();
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private GetClubIsFollowerQueryHandler CreateHandler()
        => new(
            _userClubRepository.Object,
            _currentUserServices.Object,
            _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Return_Followers_Successfully_When_User_Is_Club_Admin()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var mockImageUrl = "https://cdn.unitrack.com/profile.jpg";

        // Handler içindeki rolleri ve id kontrollerini simüle ediyoruz
        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid()); // Herhangi bir user id
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);

        var department = new Department { Name = "Bilgisayar Mühendisliği" };

        var followers = new List<UserClub>
        {
            new UserClub
            {
                User = new User
                {
                    UserDetail = new UserDetail
                    {
                        Name = "Ali",
                        Surname = "Yılmaz",
                        Department = department,
                        UniverstiyId = Guid.NewGuid(),
                        ProfileImageUrl = mockImageUrl // Handler bunu bekliyor
                    }
                }
            }
        };

        _userClubRepository
            .Setup(x => x.GetClubFollowersAsync(clubId))
            .ReturnsAsync(followers);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetClubIsFollowerQuery { ClubId = clubId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data[0].Name.Should().Be("Ali");
        result.Data[0].Department.Should().Be("Bilgisayar Mühendisliği");
        result.Data[0].ImageUrl.Should().Be(mockImageUrl); // Handler ImageUrl atıyor
    }

    [Fact]
    public async Task Handle_Should_Return_NotAuthorized_When_Role_Is_User()
    {
        // Arrange
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);
        _localizationService.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync("Yetkisiz Erişim");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetClubIsFollowerQuery { ClubId = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Yetkisiz Erişim");
    }
}