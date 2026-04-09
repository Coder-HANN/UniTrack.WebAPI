using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.Profile.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class ClubProfileUpdateCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock = new();
    private readonly Mock<IClubRepository> _clubRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();
    private readonly Mock<IPasswordHasher<Club>> _passwordHasherMock = new();

    private readonly ClubProfileUpdateCommandHandler _handler;

    public ClubProfileUpdateCommandHandlerTests()
    {
        _handler = new ClubProfileUpdateCommandHandler(
            _currentUserMock.Object,
            _clubRepoMock.Object,
            _userRepoMock.Object,
            _localizationMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_UnauthorizedRole_ReturnsError()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User); // User rolü yetkisiz
        _localizationMock.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync("Unauthorized");

        // Act
        var result = await _handler.Handle(new ClubProfileUpdateCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_EmailAlreadyUsed_ByOtherClub_ReturnsError()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var existingClub = new Club { Id = clubId, PresidentMail = "old@mail.com" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(existingClub);

        // Başka bir kulüp bu maili kullanıyor
        _clubRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Club>
        {
            new Club { Id = Guid.NewGuid(), PresidentMail = "taken@mail.com" }
        });
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());
        _localizationMock.Setup(x => x.Get(ValidationKeys.EmailAlreadyUsed)).ReturnsAsync("Mail Taken");

        var command = new ClubProfileUpdateCommand { PresidentMail = "taken@mail.com" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Mail Taken");
    }

    [Fact]
    public async Task Handle_PasswordChange_ShouldFail_WhenCurrentPasswordIncorrect()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var club = new Club { Id = clubId, Password = "hashed_old_password" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(club);
        _clubRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Club>());
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

        // Şifre doğrulama başarısız
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, club.Password, "wrong_password"))
            .Returns(PasswordVerificationResult.Failed);

        _localizationMock.Setup(x => x.Get(ValidationKeys.CurrentPasswordIncorrect)).ReturnsAsync("Incorrect Password");

        var command = new ClubProfileUpdateCommand
        {
            NowPassword = "wrong_password",
            Password = "new_password_123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Incorrect Password");
    }

    [Fact]
    public async Task Handle_PasswordChange_ShouldFail_WhenNewPasswordIsSameAsOld()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var club = new Club { Id = clubId, Password = "hashed_password" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(club);
        _clubRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Club>());
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

        // Mevcut şifre doğru ama yeni şifre de eskisiyle aynı
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, club.Password, "same_pass"))
            .Returns(PasswordVerificationResult.Success);

        _localizationMock.Setup(x => x.Get(ValidationKeys.NewPasswordCannotBeSameAsOld)).ReturnsAsync("Same Password Error");

        var command = new ClubProfileUpdateCommand { NowPassword = "same_pass", Password = "same_pass" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Same Password Error");
    }

    [Fact]
    public async Task Handle_SuccessfulUpdate_IncludingPassword_UpdatesClub()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var club = new Club { Id = clubId, Name = "Old Name", Password = "old_hash" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(club);
        _clubRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Club>());
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

        // Şifre doğrulama başarılı (mevcut şifre için)
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, "old_hash", "current_pass"))
            .Returns(PasswordVerificationResult.Success);

        // Yeni şifre kontrolü (farklı olduğu için failed dönerse logic ilerler)
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, "old_hash", "new_pass"))
            .Returns(PasswordVerificationResult.Failed);

        _passwordHasherMock.Setup(x => x.HashPassword(club, "new_pass")).Returns("new_hashed_pass");

        _localizationMock.Setup(x => x.Get(ValidationKeys.ProfileUpdatedSuccessfully, It.IsAny<object>()))
            .ReturnsAsync("Success");

        var command = new ClubProfileUpdateCommand
        {
            Name = "New Name",
            NowPassword = "current_pass",
            Password = "new_pass"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        club.Name.Should().Be("New Name");
        club.Password.Should().Be("new_hashed_pass");
        _clubRepoMock.Verify(x => x.UpdateAsync(club), Times.Once);
    }
}