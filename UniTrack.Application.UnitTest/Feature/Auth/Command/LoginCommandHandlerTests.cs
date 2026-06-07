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
using UniTrack.Application.Common;
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
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);
        _localizationMock.Setup(x => x.Get(ValidationKeys.NotAuthorized)).ReturnsAsync("Unauthorized");

        var result = await _handler.Handle(new ClubProfileUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_EmailAlreadyUsed_ByOtherClub_ReturnsError()
    {
        var clubId = Guid.NewGuid();
        var existingClub = new Club { Id = clubId, PresidentMail = "old@mail.com", Name = "Old Name" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(existingClub);
        _clubRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Club> { new Club { Id = Guid.NewGuid(), PresidentMail = "taken@mail.com" } });
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());
        _localizationMock.Setup(x => x.Get(ValidationKeys.EmailAlreadyUsed)).ReturnsAsync("Mail Taken");

        var command = new ClubProfileUpdateCommand { Name = "Old Name", PresidentMail = "taken@mail.com" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Mail Taken");
    }

    [Fact]
    public async Task Handle_PasswordChange_ShouldFail_WhenCurrentPasswordIncorrect()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var club = new Club { Id = clubId, Name = "Old Name", Password = "hashed_old_password" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(club);

        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, club.Password, "wrong_password"))
            .Returns(PasswordVerificationResult.Failed);

        _localizationMock.Setup(x => x.Get(ValidationKeys.CurrentPasswordIncorrect)).ReturnsAsync("Incorrect Password");

        // Temel alan değiştirilmiyor (Name atlanıyor veya null bırakılıyor), böylece isUpdated false kalıyor ve üstteki şifre kontrol bloğu hata fırlatıyor.
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
        var club = new Club { Id = clubId, Name = "Old Name", Password = "hashed_password" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(club);

        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, club.Password, "same_pass"))
            .Returns(PasswordVerificationResult.Success);

        _localizationMock.Setup(x => x.Get(ValidationKeys.NewPasswordCannotBeSameAsOld)).ReturnsAsync("Same Password Error");

        var command = new ClubProfileUpdateCommand
        {
            NowPassword = "same_pass",
            Password = "same_pass"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Same Password Error");
    }

    [Fact]
    public async Task Handle_SuccessfulUpdate_IncludingPassword_UpdatesClub()
    {
        var clubId = Guid.NewGuid();
        var club = new Club { Id = clubId, Name = "Old Name", Password = "old_hash" };

        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);
        _clubRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>())).ReturnsAsync(club);
        _clubRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Club>());
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, "old_hash", "current_pass")).Returns(PasswordVerificationResult.Success);
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(club, "old_hash", "new_pass")).Returns(PasswordVerificationResult.Failed);
        _passwordHasherMock.Setup(x => x.HashPassword(club, "new_pass")).Returns("new_hashed_pass");
        _localizationMock.Setup(x => x.Get(ValidationKeys.ProfileUpdatedSuccessfully, It.IsAny<object>())).ReturnsAsync("Success");

        var command = new ClubProfileUpdateCommand
        {
            Name = "New Name",
            NowPassword = "current_pass",
            Password = "new_pass"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _clubRepoMock.Verify(x => x.UpdateAsync(club), Times.Once);
    }
}