using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Profile.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;
using Microsoft.AspNetCore.Identity;

public class UserProfileUpdateCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUser = new();
    private readonly Mock<IUserDetailRepository> _userDetailRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ILocalizationService> _localization = new();
    private readonly Mock<IPasswordHasher<User>> _passwordHasher = new();

    private readonly UserProfileUpdateCommandHandler _handler;

    public UserProfileUpdateCommandHandlerTests()
    {
        _handler = new UserProfileUpdateCommandHandler(
            _currentUser.Object,
            _userDetailRepo.Object,
            _userRepo.Object,
            _localization.Object,
            _passwordHasher.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        _currentUser.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _localization.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync("Unauthorized");

        // Act
        var result = await _handler.Handle(new UserProfileUpdateCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ReturnsError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingEmail = "exists@mail.com";

        _currentUser.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUser.Setup(x => x.Role()).Returns(Role.User);

        _userDetailRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserDetail, bool>>>()))
            .ReturnsAsync(new UserDetail { UserId = userId, Name = "Old" });

        _userRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new User { Id = userId, Email = "old@mail.com" });

        // Email çakışması için başka bir kullanıcı simülesi
        _userRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<User> { new User { Id = Guid.NewGuid(), Email = existingEmail } });

        _localization.Setup(x => x.Get(ValidationKeys.UserEmailAlreadyExists)).ReturnsAsync("Mail exists");

        var command = new UserProfileUpdateCommand { Email = existingEmail };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Mail exists");
    }

    [Fact]
    public async Task Handle_PasswordChange_Success_When_Correct_NowPassword_Provided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Password = "hashed_old_password" };
        var command = new UserProfileUpdateCommand
        {
            NowPassword = "plain_old_password",
            Password = "new_secure_password"
        };

        _currentUser.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUser.Setup(x => x.Role()).Returns(Role.User);

        _userRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
        _userDetailRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserDetail, bool>>>()))
            .ReturnsAsync(new UserDetail { UserId = userId });

        // Mevcut şifre doğrulaması başarılı
        _passwordHasher.Setup(x => x.VerifyHashedPassword(user, user.Password, command.NowPassword))
            .Returns(PasswordVerificationResult.Success);

        // Yeni şifre eskisinden farklı (Verify başarısız dönerse farklı demektir)
        _passwordHasher.Setup(x => x.VerifyHashedPassword(user, user.Password, command.Password))
            .Returns(PasswordVerificationResult.Failed);

        _passwordHasher.Setup(x => x.HashPassword(user, command.Password))
            .Returns("new_hashed_password");

        _localization.Setup(x => x.Get(ValidationKeys.ProfileUpdatedSuccessfully)).ReturnsAsync("Success");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Password.Should().Be("new_hashed_password");
        _userRepo.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUpdate_NoPassword_UpdatesProfileSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDetail = new UserDetail { UserId = userId, Name = "Old" };
        var user = new User { Id = userId, Email = "old@mail.com" };

        _currentUser.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUser.Setup(x => x.Role()).Returns(Role.User);

        _userDetailRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserDetail, bool>>>())).ReturnsAsync(userDetail);
        _userRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
        _userRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());
        _localization.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync("Success");

        var command = new UserProfileUpdateCommand { Name = "NewName", Surname = "NewSurname" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        userDetail.Name.Should().Be("NewName");
        _userDetailRepo.Verify(x => x.UpdateAsync(userDetail), Times.Once);
    }
}