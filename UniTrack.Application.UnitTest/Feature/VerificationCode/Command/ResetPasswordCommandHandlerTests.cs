using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVerificationCodeService> _codeServiceMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly Mock<ILocalizationService> _localizationMock;

    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _codeServiceMock = new Mock<IVerificationCodeService>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new ResetPasswordCommandHandler(
            _userRepositoryMock.Object,
            _codeServiceMock.Object,
            _passwordHasherMock.Object,
            _localizationMock.Object
        );
    }

    [Fact]
    public async Task Handle_InvalidCode_ReturnsFail()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            Email = "user@mail.com",
            Code = "wrong",
            NewPassword = "123"
        };

        _codeServiceMock
            .Setup(x => x.ValidateCode(command.Email, command.Code, VerificationType.PasswordReset))
            .Returns(false);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.InvalidOrExpiredCode))
            .ReturnsAsync("Invalid code");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid code", result.Message);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFail()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            Email = "user@mail.com",
            Code = "123456",
            NewPassword = "newpass"
        };

        _codeServiceMock
            .Setup(x => x.ValidateCode(command.Email, command.Code, VerificationType.PasswordReset))
            .Returns(true);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync((User)null);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.UserNotFound))
            .ReturnsAsync("User not found");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_ResetsPassword_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Email = "user@mail.com" };

        var command = new ResetPasswordCommand
        {
            Email = user.Email,
            Code = "123456",
            NewPassword = "newpass"
        };

        _codeServiceMock
            .Setup(x => x.ValidateCode(command.Email, command.Code, VerificationType.PasswordReset))
            .Returns(true);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.HashPassword(null, command.NewPassword))
            .Returns("hashed-password");

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.PasswordChangedSuccess))
            .ReturnsAsync("Password changed");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Password changed", result.Message);
        Assert.Equal("hashed-password", user.Password);

        _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
        _codeServiceMock.Verify(
            x => x.RemoveCode(command.Email, VerificationType.PasswordReset),
            Times.Once
        );
    }
}
