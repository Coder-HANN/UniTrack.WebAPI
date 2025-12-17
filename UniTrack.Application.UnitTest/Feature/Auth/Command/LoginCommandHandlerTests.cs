using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Application.Feature.Auth.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<IPasswordHasher<User>> _userPasswordHasher;
    private readonly Mock<IPasswordHasher<Club>> _clubPasswordHasher;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _clubRepository = new Mock<IClubRepository>();
        _userPasswordHasher = new Mock<IPasswordHasher<User>>();
        _clubPasswordHasher = new Mock<IPasswordHasher<Club>>();
        _configuration = new Mock<IConfiguration>();
        _localizationService = new Mock<ILocalizationService>();

        SetupJwtConfiguration();

        _handler = new LoginCommandHandler(
            _userRepository.Object,
            _clubRepository.Object,
            _userPasswordHasher.Object,
            _clubPasswordHasher.Object,
            _configuration.Object,
            _localizationService.Object);
    }

    // -------------------------
    // ✅ USER LOGIN SUCCESS
    // -------------------------
    [Fact]
    public async Task Handle_Should_Login_User_Successfully()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            Password = "hashed",
            Role = Role.User,
            UserDetail = new UserDetail { Name = "Ali", Surname = "Veli" }
        };

        _userRepository.Setup(x => x.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(user, user.Password, "123456"))
            .Returns(PasswordVerificationResult.Success);

        _localizationService.Setup(x => x.Get(ValidationKeys.LoginSuccess))
            .ReturnsAsync("Login successful");

        // Act
        var result = await _handler.Handle(
            new LoginCommand { Email = user.Email, Password = "123456" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data.Token);

        _userRepository.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    // -------------------------
    // ✅ CLUB LOGIN SUCCESS
    // -------------------------
    [Fact]
    public async Task Handle_Should_Login_Club_Successfully()
    {
        // Arrange
        var club = new Club
        {
            Id = Guid.NewGuid(),
            PresidentMail = "club@test.com",
            Password = "hashed",
            Role = Role.Club,
            Name = "Aviation Club"
        };

        _clubRepository.Setup(x => x.GetByEmailAsync(club.PresidentMail))
            .ReturnsAsync(club);

        _clubPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(club, club.Password, "123456"))
            .Returns(PasswordVerificationResult.Success);

        _localizationService.Setup(x => x.Get(ValidationKeys.LoginSuccess))
            .ReturnsAsync("Login successful");

        // Act
        var result = await _handler.Handle(
            new LoginCommand { Email = club.PresidentMail, Password = "123456" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data.Token);

        _clubRepository.Verify(x => x.UpdateAsync(club), Times.Once);
    }

    // -------------------------
    // ❌ INVALID PASSWORD
    // -------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Password_Is_Invalid()
    {
        // Arrange
        var user = new User
        {
            Email = "user@test.com",
            Password = "hashed",
            Role = Role.User
        };

        _userRepository.Setup(x => x.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(user, user.Password, "wrong"))
            .Returns(PasswordVerificationResult.Failed);

        _localizationService.Setup(x => x.Get(ValidationKeys.InvalidEmailOrPassword))
            .ReturnsAsync("Invalid email or password");

        // Act
        var result = await _handler.Handle(
            new LoginCommand { Email = user.Email, Password = "wrong" },
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password", result.Message);
    }

    // -------------------------
    // ❌ USER NOT FOUND
    // -------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_User_Not_Found()
    {
        // Arrange
        _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);

        _clubRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Club)null);

        _localizationService.Setup(x => x.Get(ValidationKeys.InvalidEmailOrPassword))
            .ReturnsAsync("Invalid email or password");

        // Act
        var result = await _handler.Handle(
            new LoginCommand { Email = "none@test.com", Password = "123" },
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    // -------------------------
    // JWT CONFIG MOCK
    // -------------------------
    private void SetupJwtConfiguration()
    {
        var jwtSettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "THIS_IS_A_TEST_SECRET_KEY_1234567890" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(jwtSettings)
            .Build();

        _configuration.Setup(x => x.GetSection("Jwt"))
            .Returns(config.GetSection("Jwt"));
    }
}
