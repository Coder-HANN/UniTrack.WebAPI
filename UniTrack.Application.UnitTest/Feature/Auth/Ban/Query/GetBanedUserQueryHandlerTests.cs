using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
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
    // ❌ ROLE USER → UNAUTHORIZED (via localization)
    // --------------------------------------------------
    // NOTE: The handler does NOT have a separate null-user guard.
    // A null CurrentUser() still falls through to the role != Admin check.
    // The unauthorized message comes from localizationService.Get(ValidationKeys.NotAuthorized).
    [Theory]
    [InlineData(Role.User)]
    [InlineData(Role.Club)]
    public async Task Handle_Should_Fail_When_Role_Is_Not_Admin(Role role)
    {
        // Arrange
        const string unauthorizedMessage = "Yetkisiz kullanıcı";

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(role);

        _localization
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync(unauthorizedMessage);

        // Act
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(unauthorizedMessage, result.Message);

        // Repository must never be called when unauthorized
        _banRepository.Verify(
            x => x.GetBannedUserInUniversityAsync(It.IsAny<Guid?>()),
            Times.Never);
    }

    // --------------------------------------------------
    // ❌ NULL CurrentUser → UNAUTHORIZED (role check fires)
    // --------------------------------------------------
    // The handler reads role regardless of whether CurrentUser() is null,
    // so a null user with a non-Admin role hits the same unauthorized branch.
    [Fact]
    public async Task Handle_Should_Fail_When_CurrentUser_Is_Null_And_Role_Is_Not_Admin()
    {
        // Arrange
        const string unauthorizedMessage = "Yetkisiz kullanıcı";

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns((Guid?)null);

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.User);

        _localization
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync(unauthorizedMessage);

        // Act
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(unauthorizedMessage, result.Message);
    }

    // --------------------------------------------------
    // ❌ BAN LIST NULL → LİSTE BOŞ
    // --------------------------------------------------
    // The repository method is GetBannedUserInUniversityAsync(UniversityId()),
    // NOT GetAllAsync() — it's scoped to the current user's university.
    [Fact]
    public async Task Handle_Should_Fail_When_Ban_List_Is_Null()
    {
        // Arrange
        var universityId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _currentUserServices
            .Setup(x => x.UniversityId())
            .Returns(universityId);

        _banRepository
            .Setup(x => x.GetBannedUserInUniversityAsync(universityId))
            .ReturnsAsync((List<Ban>)null);

        // Act
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Liste boş", result.Message);
    }

    // --------------------------------------------------
    // ✅ ADMIN → BANLI KULLANICILAR DÖNER
    // --------------------------------------------------
    // The DTO also maps Name (from User.UserDetail.Name) and Description (from Ban.Description).
    // The success message comes from localizationService.Get(ValidationKeys.OperationSuccessful).
    [Fact]
    public async Task Handle_Should_Return_Banned_Users_When_Admin()
    {
        // Arrange
        const string successMessage = "Başarılı";
        var universityId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var banId = Guid.NewGuid();
        var createdDate = DateTime.UtcNow.AddDays(-1);
        var lastDate = DateTime.UtcNow.AddDays(10);

        var banList = new List<Ban>
        {
            new Ban
            {
                Id = banId,
                UserId = userId,
                Description = "Kural ihlali",
                CreatedDate = createdDate,
                LastDate = lastDate,
                User = new User
                {
                    Id = userId,
                    Role = Role.User,
                    UserDetail = new UserDetail
                    {
                        Name = "Ali Veli"
                    }
                }
            }
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _currentUserServices
            .Setup(x => x.UniversityId())
            .Returns(universityId);

        _banRepository
            .Setup(x => x.GetBannedUserInUniversityAsync(universityId))
            .ReturnsAsync(banList);

        _localization
            .Setup(x => x.Get(ValidationKeys.OperationSuccessful))
            .ReturnsAsync(successMessage);

        // Act
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal(successMessage, result.Message);

        var item = result.Data[0];
        Assert.Equal(banId, item.Id);
        Assert.Equal(userId, item.UserId);
        Assert.Equal(Role.User, item.Role);
        Assert.Equal("Ali Veli", item.Name);
        Assert.Equal("Kural ihlali", item.Description);
        Assert.Equal(createdDate, item.CreatedDate);
        Assert.Equal(lastDate, item.LastDate);
    }

    // --------------------------------------------------
    // ✅ ADMIN, UNIVERSITY SCOPED → DOĞRU UNİVERSİTE ID GÖNDERİLİR
    // --------------------------------------------------
    // Verifies the handler passes the correct UniversityId to the repository,
    // ensuring data isolation between universities.
    [Fact]
    public async Task Handle_Should_Pass_UniversityId_To_Repository()
    {
        // Arrange
        var universityId = Guid.NewGuid();

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _currentUserServices
            .Setup(x => x.UniversityId())
            .Returns(universityId);

        _banRepository
            .Setup(x => x.GetBannedUserInUniversityAsync(universityId))
            .ReturnsAsync(new List<Ban>());

        _localization
            .Setup(x => x.Get(ValidationKeys.OperationSuccessful))
            .ReturnsAsync("Başarılı");

        // Act
        await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert — repository must be called with the exact university ID
        _banRepository.Verify(
            x => x.GetBannedUserInUniversityAsync(universityId),
            Times.Once);
    }

    // --------------------------------------------------
    // ✅ ADMIN, BOŞ LİSTE → BAŞARILI AMA VERİ YOK
    // --------------------------------------------------
    // An empty (non-null) list is valid: returns success with an empty Data list.
    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Bans_Exist()
    {
        // Arrange
        var universityId = Guid.NewGuid();
        const string successMessage = "Başarılı";

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _currentUserServices
            .Setup(x => x.UniversityId())
            .Returns(universityId);

        _banRepository
            .Setup(x => x.GetBannedUserInUniversityAsync(universityId))
            .ReturnsAsync(new List<Ban>());

        _localization
            .Setup(x => x.Get(ValidationKeys.OperationSuccessful))
            .ReturnsAsync(successMessage);

        // Act
        var result = await _handler.Handle(new GetBanedUserQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(successMessage, result.Message);
    }
}