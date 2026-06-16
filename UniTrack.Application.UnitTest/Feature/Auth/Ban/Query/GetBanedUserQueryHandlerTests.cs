using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.DTOs.Ban;
using UniTrack.Application.Feature.Ban.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class GetBanedUserQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IBanRepository> _banRepository;
    private readonly Mock<ILocalizationService> _localization;

    private readonly GetBanedUserQueryHandler _handler;

    public GetBanedUserQueryHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _banRepository = new Mock<IBanRepository>();
        _localization = new Mock<ILocalizationService>();

        _handler = new GetBanedUserQueryHandler(
            _currentUserServices.Object,
            _banRepository.Object,
            _localization.Object);
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
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

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
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

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
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Liste boş", result.Message);
        Assert.Null(result.Data);
    }

    // --------------------------------------------------
    // ✅ ADMIN → BANLI KULLANICILAR DÖNER
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Return_Banned_Users_When_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var banList = new List<Ban>
        {
            new Ban
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                LastDate = DateTime.UtcNow.AddDays(10),
                User = new User
                {
                    Id = userId,
                    Role = Role.User
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
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var item = result.Data[0];
        Assert.Equal(userId, item.UserId);
        Assert.Equal(Role.User, item.Role);
        Assert.Equal("Başarılı", result.Message);
    }
}
