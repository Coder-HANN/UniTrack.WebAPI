using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Ban.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class GetBanedClubQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IBanRepository> _banRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly GetBanedClubQueryHandler _handler;

    private const string NotAuthorizedMessage = "Yetkisiz kullanıcı";
    private const string OperationFailedMessage = "İşlem başarısız";
    private const string OperationSuccessMessage = "Başarılı";

    public GetBanedClubQueryHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _banRepository = new Mock<IBanRepository>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new GetBanedClubQueryHandler(
            _currentUserServices.Object,
            _banRepository.Object,
            _localizationService.Object);

        // Global localization setup — her key için sabit bir string döndür
        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync(NotAuthorizedMessage);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.OperationFailed))
            .ReturnsAsync(OperationFailedMessage);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.OperationSuccessful))
            .ReturnsAsync(OperationSuccessMessage);
    }

    // --------------------------------------------------
    // ❌ CURRENT USER NULL → UNAUTHORIZED
    // --------------------------------------------------
    // Handler'da userId == null için ayrı bir guard var,
    // localizationService.Get(ValidationKeys.NotAuthorized) çağırıyor.
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
        Assert.Null(result.Data);
        Assert.Equal(NotAuthorizedMessage, result.Message);

        // Repository hiç çağrılmamalı
        _banRepository.Verify(
            x => x.GetBannedClubInUniversityAsync(It.IsAny<Guid?>()),
            Times.Never);
    }

    // --------------------------------------------------
    // ❌ ROLE USER / CLUB → UNAUTHORIZED
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
        Assert.Null(result.Data);
        Assert.Equal(NotAuthorizedMessage, result.Message);

        _banRepository.Verify(
            x => x.GetBannedClubInUniversityAsync(It.IsAny<Guid?>()),
            Times.Never);
    }

    // --------------------------------------------------
    // ❌ BAN LIST NULL → BAŞARISIZ
    // --------------------------------------------------
    // Handler GetBannedClubInUniversityAsync(UniversityId()) çağırıyor,
    // GetAllAsync() DEĞİL — mock da buna göre kurulmalı.
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

        // ✅ Doğru metod: GetBannedClubInUniversityAsync (GetAllAsync değil)
        _banRepository
            .Setup(x => x.GetBannedClubInUniversityAsync(universityId))
            .ReturnsAsync((List<Ban>)null);

        // Act
        var result = await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OperationFailedMessage, result.Message);
    }

    // --------------------------------------------------
    // ✅ ADMIN → BANLI KULÜPLER DÖNER
    // --------------------------------------------------
    // Handler GetBannedClubInUniversityAsync(UniversityId()) çağırıyor.
    // DTO: Id, ClubId, Role, Name, Description, CreatedDate, LastDate
    [Fact]
    public async Task Handle_Should_Return_Banned_Clubs_When_Admin()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var banId = Guid.NewGuid();
        var universityId = Guid.NewGuid();
        var createdDate = DateTime.UtcNow.AddDays(-2);
        var lastDate = DateTime.UtcNow.AddDays(5);

        var banList = new List<Ban>
        {
            new Ban
            {
                Id          = banId,
                ClubId      = clubId,
                Description = "Kural ihlali",
                CreatedDate = createdDate,
                LastDate    = lastDate,
                Club = new Club
                {
                    Id           = clubId,
                    Role         = Role.Club,
                    Name         = "Test Kulübü",
                    UniversityId = universityId
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

        // ✅ Doğru metod: GetBannedClubInUniversityAsync (GetAllAsync değil)
        _banRepository
            .Setup(x => x.GetBannedClubInUniversityAsync(universityId))
            .ReturnsAsync(banList);

        // Act
        var result = await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal(OperationSuccessMessage, result.Message);

        var item = result.Data[0];
        Assert.Equal(banId, item.Id);
        Assert.Equal(clubId, item.ClubId);
        Assert.Equal(Role.Club, item.Role);
        Assert.Equal("Test Kulübü", item.Name);
        Assert.Equal("Kural ihlali", item.Description);
        Assert.Equal(createdDate, item.CreatedDate);
        Assert.Equal(lastDate, item.LastDate);
    }

    // --------------------------------------------------
    // ✅ ADMIN, DOĞRU UNİVERSİTE ID GÖNDERİLİR
    // --------------------------------------------------
    // Repository'nin tam olarak UniversityId() değeriyle çağrıldığını doğrular.
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
            .Setup(x => x.GetBannedClubInUniversityAsync(universityId))
            .ReturnsAsync(new List<Ban>());

        // Act
        await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert — tam olarak bu ID ile çağrılmalı
        _banRepository.Verify(
            x => x.GetBannedClubInUniversityAsync(universityId),
            Times.Once);
    }

    // --------------------------------------------------
    // ✅ ADMIN, BOŞ LİSTE → BAŞARILI AMA VERİ YOK
    // --------------------------------------------------
    // null liste → hata; boş liste → başarı (Data = [])
    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Bans_Exist()
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
            .Setup(x => x.GetBannedClubInUniversityAsync(universityId))
            .ReturnsAsync(new List<Ban>());

        // Act
        var result = await _handler.Handle(new GetBanedClubQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(OperationSuccessMessage, result.Message);
    }
}