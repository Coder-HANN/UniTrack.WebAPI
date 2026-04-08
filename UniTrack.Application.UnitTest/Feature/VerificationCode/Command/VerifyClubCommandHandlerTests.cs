using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class VerifyCommandHandlerTests
{
    private readonly Mock<IClubRepository> _clubRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVerificationCodeService> _codeServiceMock;
    private readonly Mock<ILocalizationService> _localizationMock;
    private readonly VerifyCommandHandler _handler;

    public VerifyCommandHandlerTests()
    {
        _clubRepositoryMock = new Mock<IClubRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _codeServiceMock = new Mock<IVerificationCodeService>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new VerifyCommandHandler(
            _clubRepositoryMock.Object,
            _userRepositoryMock.Object,
            _codeServiceMock.Object,
            _localizationMock.Object
        );
    }

    #region Club Registration Tests

    [Fact]
    public async Task Handle_ClubRegistration_ClubNotFound_ReturnsFail()
    {
        // Arrange
        var command = new VerificationCommand { Email = "club@test.com", VerificationType = VerificationType.ClubRegistration };
        _clubRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync((Club)null);
        _localizationMock.Setup(x => x.Get(ValidationKeys.ClubNotFound)).ReturnsAsync("Club not found");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Club not found", result.Message);
    }

    [Fact]
    public async Task Handle_ClubRegistration_InvalidCode_DeletesClub_ReturnsFail()
    {
        // Arrange
        var club = new Club { PresidentMail = "club@test.com" };
        var command = new VerificationCommand { Email = club.PresidentMail, VerificationCode = "123", VerificationType = VerificationType.ClubRegistration };

        _clubRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(club);
        _codeServiceMock.Setup(x => x.ValidateCode(command.Email, command.VerificationCode, VerificationType.ClubRegistration)).Returns(false);
        _localizationMock.Setup(x => x.Get(ValidationKeys.InvalidOrExpiredCode)).ReturnsAsync("Invalid code");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _clubRepositoryMock.Verify(x => x.DeleteAsync(club), Times.Once);
    }

    [Fact]
    public async Task Handle_ClubRegistration_ValidCode_VerifiesClub_ReturnsSuccess()
    {
        // Arrange
        var club = new Club { PresidentMail = "club@test.com", IsVerified = false };
        var command = new VerificationCommand { Email = club.PresidentMail, VerificationCode = "123456", VerificationType = VerificationType.ClubRegistration };

        _clubRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(club);
        _codeServiceMock.Setup(x => x.ValidateCode(command.Email, command.VerificationCode, VerificationType.ClubRegistration)).Returns(true);
        _localizationMock.Setup(x => x.Get(ValidationKeys.ClubVerifiedSuccess)).ReturnsAsync("Success");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(club.IsVerified);
        _clubRepositoryMock.Verify(x => x.UpdateAsync(club), Times.Once);
        _codeServiceMock.Verify(x => x.RemoveCode(command.Email, VerificationType.ClubRegistration), Times.Once);
    }

    #endregion

    #region User Registration Tests

    [Fact]
    public async Task Handle_UserRegistration_UserNotFound_ReturnsFail()
    {
        // Arrange
        var command = new VerificationCommand { Email = "user@test.com", VerificationType = VerificationType.UserRegistration };
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync((User)null);
        _localizationMock.Setup(x => x.Get(ValidationKeys.UserNotFound)).ReturnsAsync("User not found");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Message);
    }

    [Fact]
    public async Task Handle_UserRegistration_InvalidCode_DeletesUser_ReturnsFail()
    {
        // Arrange
        var user = new User { Email = "user@test.com" };
        var command = new VerificationCommand { Email = user.Email, VerificationCode = "000", VerificationType = VerificationType.UserRegistration };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _codeServiceMock.Setup(x => x.ValidateCode(command.Email, command.VerificationCode, VerificationType.UserRegistration)).Returns(false);
        _localizationMock.Setup(x => x.Get(ValidationKeys.InvalidOrExpiredCode)).ReturnsAsync("Invalid code");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _userRepositoryMock.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_UserRegistration_ValidCode_VerifiesUser_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Email = "user@test.com", IsVerified = false };
        var command = new VerificationCommand { Email = user.Email, VerificationCode = "123456", VerificationType = VerificationType.UserRegistration };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _codeServiceMock.Setup(x => x.ValidateCode(command.Email, command.VerificationCode, VerificationType.UserRegistration)).Returns(true);
        _localizationMock.Setup(x => x.Get(ValidationKeys.OperationSuccessful)).ReturnsAsync("Success");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(user.IsVerified);
        _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
        _codeServiceMock.Verify(x => x.RemoveCode(command.Email, VerificationType.UserRegistration), Times.Once);
    }

    #endregion

    [Fact]
    public async Task Handle_UnknownVerificationType_ReturnsInvalidRequest()
    {
        // Arrange
        var command = new VerificationCommand { VerificationType = (VerificationType)999 };
        _localizationMock.Setup(x => x.Get(ValidationKeys.InvalidRequest)).ReturnsAsync("Invalid request");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid request", result.Message);
    }
}