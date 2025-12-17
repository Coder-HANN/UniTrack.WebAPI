using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Feature.Ban.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class BanForClubCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<IBanRepository> _banRepository;

    private readonly BanForClubCommandHandler _handler;

    public BanForClubCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _clubRepository = new Mock<IClubRepository>();
        _banRepository = new Mock<IBanRepository>();

        _handler = new BanForClubCommandHandler(
            _currentUserServices.Object,
            _clubRepository.Object,
            _banRepository.Object);
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
        Assert.Equal("Unauthorized", result.Message);

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
        Assert.Equal("Yetkisiz kullanıcı", result.Message);

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
            .Returns(Role.Admin);

        _clubRepository
            .Setup(x => x.GetByIdAsync(command.ClubId))
            .ReturnsAsync((Club)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Club not found", result.Message);

        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ ADMIN & CLUB VAR → BAN BAŞARILI
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Ban_Club_Successfully_When_Admin()
    {
        // Arrange
        var clubId = Guid.NewGuid();

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

        _clubRepository
            .Setup(x => x.GetByIdAsync(clubId))
            .ReturnsAsync(new Club { Id = clubId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Club banned successfully", result.Message);

        _banRepository.Verify(x => x.AddAsync(It.Is<Ban>(b =>
            b.ClubId == clubId &&
            b.Role == Role.Club &&
            b.IsBanned == true &&
            b.Description == command.Description
        )), Times.Once);
    }
}
