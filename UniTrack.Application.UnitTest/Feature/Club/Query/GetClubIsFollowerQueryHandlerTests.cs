using Moq;
using Xunit;
using FluentAssertions;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

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
    public async Task Handle_Should_Return_Followers_Successfully()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.Club);

        var department = new Department
        {
            Id = 5,
            Name = "Bilgisayar Mühendisliği"
        };

        var imageBytes = new byte() ;

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
                        ProfileImage = imageBytes
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
            new GetClubIsFollowerQuery(clubId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data[0].Department.Should().Be("Bilgisayar Mühendisliği");
        result.Data[0].Image.Should().Equals(imageBytes);
    }
}
