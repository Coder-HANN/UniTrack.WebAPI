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

public class BanForUserCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IBanRepository> _banRepository;

    private readonly BanForUserCommandHandler _handler;

    public BanForUserCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _userRepository = new Mock<IUserRepository>();
        _banRepository = new Mock<IBanRepository>();

        _handler = new BanForUserCommandHandler(
            _currentUserServices.Object,
            _userRepository.Object,
            _banRepository.Object);
    }

    // --------------------------------------------------
    // ❌ CURRENT USER NULL → UNAUTHORIZED
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_CurrentUser_Is_Null()
    {
        // Arrange
        var command = new BanForUserCommand
        {
            UserId = Guid.NewGuid()
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
        var command = new BanForUserCommand
        {
            UserId = Guid.NewGuid()
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
    // ❌ USER BULUNAMADI
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_User_Not_Found()
    {
        // Arrange
        var command = new BanForUserCommand
        {
            UserId = Guid.NewGuid()
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _userRepository
            .Setup(x => x.GetByIdAsync(command.UserId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Message);

        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ ADMIN & USER VAR → BAN BAŞARILI
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Ban_User_Successfully_When_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new BanForUserCommand
        {
            UserId = userId,
            LastDate = DateTime.UtcNow.AddDays(3),
            Description = "Topluluk kurallarına aykırı davranış"
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _userRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("User banned successfully", result.Message);

        _banRepository.Verify(x => x.AddAsync(It.Is<Ban>(b =>
            b.UserId == userId &&
            b.Role == Role.User &&
            b.IsBanned == true &&
            b.Description == command.Description
        )), Times.Once);
    }
}
