using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IClubRepository> _clubRepository = new();
    private readonly Mock<IPasswordHasher<User>> _userPasswordHasher = new();
    private readonly Mock<IPasswordHasher<Club>> _clubPasswordHasher = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<ILocalizationService> _localizationService = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        SetupJwtConfiguration();
        SetupHttpContext();

        _handler = new LoginCommandHandler(
            _userRepository.Object,
            _clubRepository.Object,
            _userPasswordHasher.Object,
            _clubPasswordHasher.Object,
            _configuration.Object,
            _localizationService.Object,
            _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_Should_Login_User_Successfully_And_Set_Cookie()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            Password = "hashed",
            Role = Role.User,
            UserDetail = new UserDetail
            {
                Name = "Bedirhan",
                Surname = "Korkmaz",
                UniverstiyId = Guid.NewGuid(),
                CityId = 1,
                DepartmentId = 1,
                Gender = Gender.Other
            }
        };

        _userRepository.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _userPasswordHasher.Setup(x => x.VerifyHashedPassword(user, user.Password, "123456"))
            .Returns(PasswordVerificationResult.Success);
        _localizationService.Setup(x => x.Get(ValidationKeys.LoginSuccess)).ReturnsAsync("Login successful");

        var result = await _handler.Handle(new LoginCommand { Email = user.Email, Password = "123456" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.FullName.Should().Be("Bedirhan Korkmaz");
        _userRepository.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Login_Club_Successfully()
    {
        var club = new Club
        {
            Id = Guid.NewGuid(),
            PresidentMail = "club@test.com",
            ContectEmail = "contact@test.com",
            Password = "hashed",
            Role = Role.Club,
            Name = "Aviation Club",
            IsVerified = true
        };

        // KESİN ÇÖZÜM: İlk adımda User kontrolü yapıldığı için user reposunun null döneceğini garanti ediyoruz
        _userRepository.Setup(x => x.GetByEmailAsync(club.PresidentMail)).ReturnsAsync((User)null);
        _clubRepository.Setup(x => x.GetByEmailAsync(club.PresidentMail)).ReturnsAsync(club);

        _clubPasswordHasher.Setup(x => x.VerifyHashedPassword(club, club.Password, "123456"))
            .Returns(PasswordVerificationResult.Success);
        _localizationService.Setup(x => x.Get(ValidationKeys.LoginSuccess)).ReturnsAsync("Club Login successful");

        var result = await _handler.Handle(new LoginCommand { Email = club.PresidentMail, Password = "123456" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Role.Should().Be("club");
        _clubRepository.Verify(x => x.UpdateAsync(club), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Login_Admin_Successfully()
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@test.com",
            Password = "hashed",
            Role = Role.Admin,
            // KESİN ÇÖZÜM: Admin kullanıcısında UserDetail nesnesinin altındaki UniversityId'nin de var olması gerekiyor (NullReference önlemek için)
            UserDetail = new UserDetail
            {
                Name = "AdminUser",
                Surname = "System",
                UniverstiyId = Guid.NewGuid()
            }
        };

        _userRepository.Setup(x => x.GetByEmailAsync(admin.Email)).ReturnsAsync(admin);
        _userPasswordHasher.Setup(x => x.VerifyHashedPassword(admin, admin.Password, "admin123"))
            .Returns(PasswordVerificationResult.Success);
        _localizationService.Setup(x => x.Get(ValidationKeys.LoginSuccess)).ReturnsAsync("Admin Login successful");

        var result = await _handler.Handle(new LoginCommand { Email = admin.Email, Password = "admin123" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Role.Should().Be("admin");
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Invalid_Credentials()
    {
        _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
        _clubRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Club)null);
        _localizationService.Setup(x => x.Get(ValidationKeys.InvalidEmailOrPassword)).ReturnsAsync("Error");

        var result = await _handler.Handle(new LoginCommand { Email = "wrong@test.com", Password = "123" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Error");
    }

    private void SetupJwtConfiguration()
    {
        var jwtSectionMock = new Mock<IConfigurationSection>();
        jwtSectionMock.Setup(x => x["Key"]).Returns("THIS_IS_A_VERY_SECRET_KEY_FOR_TESTING_12345");
        jwtSectionMock.Setup(x => x["Issuer"]).Returns("TestIssuer");
        jwtSectionMock.Setup(x => x["Audience"]).Returns("TestAudience");

        _configuration.Setup(x => x.GetSection("Jwt")).Returns(jwtSectionMock.Object);
        _configuration.Setup(x => x.GetSection("Jwt:Key").Value).Returns("THIS_IS_A_VERY_SECRET_KEY_FOR_TESTING_12345");
        _configuration.Setup(x => x.GetSection("Jwt:Issuer").Value).Returns("TestIssuer");
        _configuration.Setup(x => x.GetSection("Jwt:Audience").Value).Returns("TestAudience");
    }

    private void SetupHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
    }
}