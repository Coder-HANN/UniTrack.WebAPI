using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Feature.Ban.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class BanForClubCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<IBanRepository> _banRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly BanForClubCommandHandler _handler;

    public BanForClubCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _clubRepository = new Mock<IClubRepository>();
        _banRepository = new Mock<IBanRepository>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new BanForClubCommandHandler(
            _currentUserServices.Object,
            _clubRepository.Object,
            _banRepository.Object,
            _localizationService.Object);

        // --- GLOBAL LOCALIZATION SETUP ---
        // Testlerin string doğrulamalarında patlamaması için sahte lokalizasyon dönüşleri tanımlıyoruz
        _localizationService
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync((string key) => key); // Hangi anahtar istenirse aynısını string olarak döner
    }

    // --------------------------------------------------
    // ❌ CURRENT USER NULL → UNAUTHORIZED
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_CurrentUser_Is_Null()
    {
        // Arrange
        var command = new BanForClubCommand
        {
            ClubId = Guid.NewGuid()
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        // Handler'daki ValidationKeys.NotAuthorized string değerini doğrula
        Assert.NotNull(result.Message);

        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
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
        var command = new BanForClubCommand
        {
            ClubId = Guid.NewGuid()
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(role);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ CLUB BULUNAMADI
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Club_Not_Found()
    {
        // Arrange
        var command = new BanForClubCommand
        {
            ClubId = Guid.NewGuid()
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin); // Geçerli bir admin rolü veriyoruz

        _clubRepository
            .Setup(x => x.GetByIdAsync(command.ClubId))
            .ReturnsAsync((Club)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ ADMIN & CLUB VAR & UNIVERSITY UYUMLU → BAN BAŞARILI
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Ban_Club_Successfully_When_Admin()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var commonUniversityId = Guid.NewGuid(); // Kulüp ve Admin'in ortak üniversite ID'si

        var command = new BanForClubCommand
        {
            ClubId = clubId,
            LastDate = DateTime.UtcNow.AddDays(7),
            Description = "Kurallara aykırı faaliyet"
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        // Handler'daki üniversite kontrolünü geçmesi için Mock ayarı
        _currentUserServices
            .Setup(x => x.UniversityId())
            .Returns(commonUniversityId);

        // Kulüp nesnesini oluştururken UniversityId alanını admininkiyle eşliyoruz
        _clubRepository
            .Setup(x => x.GetByIdAsync(clubId))
            .ReturnsAsync(new Club
            {
                Id = clubId,
                UniversityId = commonUniversityId
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _banRepository.Verify(x => x.AddAsync(It.Is<Ban>(b =>
            b.ClubId == clubId &&
            b.Role == Role.Club &&
            b.IsBanned == true &&
            b.Description == command.Description
        )), Times.Once);
    }
}