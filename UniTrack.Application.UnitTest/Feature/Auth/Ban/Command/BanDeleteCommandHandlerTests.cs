using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Ban.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class BanDeleteCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IBanRepository> _banRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly BanDeleteCommandHandler _handler;

    public BanDeleteCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _banRepository = new Mock<IBanRepository>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new BanDeleteCommandHandler(
            _currentUserServices.Object,
            _banRepository.Object,
            _localizationService.Object);
    }

    // --------------------------------------------------
    // ❌ ADMIN DEĞİL → YETKİSİZ
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_User_Is_Not_Admin()
    {
        // Arrange
        var command = new BanDeleteCommand
        {
            BanId = Guid.NewGuid()
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.User);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Yetkisiz işlem");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Yetkisiz işlem", result.Message);

        _banRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _banRepository.Verify(x => x.UpdateAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ BAN BULUNAMADI
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Ban_Not_Found()
    {
        // Arrange
        var command = new BanDeleteCommand
        {
            BanId = Guid.NewGuid()
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _banRepository
            .Setup(x => x.GetByIdAsync(command.BanId))
            .ReturnsAsync((Ban)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Message);

        _banRepository.Verify(x => x.UpdateAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ BAŞARILI SOFT DELETE
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Delete_Ban_Successfully_When_Admin()
    {
        // Arrange
        var ban = new Ban
        {
            Id = Guid.NewGuid(),
            IsDeleted = false
        };

        var command = new BanDeleteCommand
        {
            BanId = ban.Id
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _banRepository
            .Setup(x => x.GetByIdAsync(ban.Id))
            .ReturnsAsync(ban);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("işlem başarılı", result.Message);
        Assert.True(ban.IsDeleted);

        _banRepository.Verify(x => x.UpdateAsync(It.Is<Ban>(b => b.IsDeleted)), Times.Once);
    }
}
