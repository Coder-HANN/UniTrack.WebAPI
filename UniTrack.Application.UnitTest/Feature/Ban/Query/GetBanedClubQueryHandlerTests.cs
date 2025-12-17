using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Application.Feature.Ban.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class GetBanedClubQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IBanRepository> _banRepository;

    private readonly GetBanedClubQueryHandler _handler;

    public GetBanedClubQueryHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _banRepository = new Mock<IBanRepository>();

        _handler = new GetBanedClubQueryHandler(
            _currentUserServices.Object,
            _banRepository.Object);
    }

    // --------------------------------------------------
    // ❌ CURRENT USER NULL → UNAUTHORIZED
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_CurrentUser_Is_Null()
    {
        // Arrange
        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Unauthorizaton", result.Message);
        Assert.Null(result.Data);
    }

    // --------------------------------------------------
    // ❌ ROLE USER / CLUB → YETKİSİZ
    // --------------------------------------------------
    [Theory]
    [InlineData(Role.User)]
    [InlineData(Role.Club)]
    public async Task Handle_Should_Fail_When_Role_Is_Not_Admin(Role role)
    {
        // Arrange
        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(role);

        // Act
        var result = await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Yetkisiz kullanıcı", result.Message);
        Assert.Null(result.Data);
    }

    // --------------------------------------------------
    // ❌ BAN LIST NULL → LİSTE BOŞ
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Ban_List_Is_Null()
    {
        // Arrange
        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _banRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync((List<Ban>)null);

        // Act
        var result = await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Liste boş", result.Message);
        Assert.Null(result.Data);
    }

    // --------------------------------------------------
    // ✅ ADMIN → BANLI CLUB LİSTESİ DÖNER
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Return_Banned_Clubs_When_Admin()
    {
        // Arrange
        var clubId = Guid.NewGuid();

        var banList = new List<Ban>
        {
            new Ban
            {
                Id = Guid.NewGuid(),
                ClubId = clubId,
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                LastDate = DateTime.UtcNow.AddDays(5),
                Club = new Club
                {
                    Id = clubId,
                    Role = Role.Club
                }
            }
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _banRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(banList);

        // Act
        var result = await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var item = result.Data[0];
        Assert.Equal(clubId, item.ClubId);
        Assert.Equal(Role.Club, item.Role);
    }
}
