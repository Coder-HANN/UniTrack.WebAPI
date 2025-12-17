using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.DTOs.Club;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Repositories.Pagenation;
using Xunit;

public class GetFollowClubQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices = new();
    private readonly Mock<IClubRepository> _clubRepository = new();
    private readonly Mock<IBaseEntityRepository<UserClub>> _baseEntityRepository = new();
    private readonly Mock<ILocalizationService> _localizationService = new();

    private GetFollowClubQueryHandler CreateHandler()
        => new(
            _currentUserServices.Object,
            _clubRepository.Object,
            _baseEntityRepository.Object,
            _localizationService.Object);

    [Fact]
    public async Task Handle_Should_Return_Followed_Clubs_With_Paging()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        var clubs = new List<Club>
        {
            new Club
            {
                Id = Guid.NewGuid(),
                Name = "Yazılım Kulübü",
                Description = "Açıklama",
                ContectEmail = "test@mail.com",
                Follower = 25,
                President = "Ali Veli",
                Tag = Tag.Hayvanseverlik,
                Logo = new byte(),
                CoverImage = new byte(),
                UserClubs = new List<UserClub>
                {
                    new UserClub { UserId = userId }
                }
            }
        };

        _clubRepository
            .Setup(x => x.GetAllClubAsync(It.IsAny<Expression<Func<Club, bool>>>()))
            .ReturnsAsync(clubs);

        var pagingResult = new PagingExecutionResult<GetFollowClubQueryResponseDTO>(
            new List<GetFollowClubQueryResponseDTO>
            {
                new GetFollowClubQueryResponseDTO
                {
                    Name = "Yazılım Kulübü",
                    Description = "Açıklama",
                    ContactMail = "test@mail.com",
                    Followers = 25,
                    President = "Ali Veli",
                    Tag = Tag.Bilim,
                    Logo = new byte(),
                    CoverImage = new byte()
                }
            },
            hasPaging: true,
            totalCount: 1,
            currentPage: 1,
            pageSize: 10
        );

        _baseEntityRepository
            .Setup(x => x.GetPagedResult(
                It.IsAny<IQueryable<GetFollowClubQueryResponseDTO>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Func<IQueryable<GetFollowClubQueryResponseDTO>, IOrderedQueryable<GetFollowClubQueryResponseDTO>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResult);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetFollowClubQuery(1, 10),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Data.Should().HaveCount(1);
        result.Data.Data.First().Tag.Should().Be(Tag.Spor);
        result.Data.Data.First().Logo.Should().NotBeNull();
    }
}
