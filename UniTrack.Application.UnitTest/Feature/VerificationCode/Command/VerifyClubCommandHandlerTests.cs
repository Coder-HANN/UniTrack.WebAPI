using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class VerifyClubCommandHandlerTests
{
    private readonly Mock<IClubRepository> _clubRepositoryMock;
    private readonly Mock<IVerificationCodeService> _codeServiceMock;
    private readonly Mock<ILocalizationService> _localizationMock;

    private readonly VerifyClubCommandHandler _handler;

    public VerifyClubCommandHandlerTests()
    {
        _clubRepositoryMock = new Mock<IClubRepository>();
        _codeServiceMock = new Mock<IVerificationCodeService>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new VerifyClubCommandHandler(
            _clubRepositoryMock.Object,
            _codeServiceMock.Object,
            _localizationMock.Object
        );
    }

    [Fact]
    public async Task Handle_ClubNotFound_ReturnsFail()
    {
        // Arrange
        var command = new VerificationCommand
        {
            Email = "club@mail.com",
            VerificationCode = "123456"
        };

        _clubRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync((Club)null);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.ClubNotFound))
            .ReturnsAsync("Club not found");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Club not found", result.Message);
    }

    [Fact]
    public async Task Handle_InvalidCode_DeletesClub_ReturnsFail()
    {
        // Arrange
        var club = new Club { PresidentMail = "club@mail.com" };

        var command = new VerificationCommand
        {
            Email = club.PresidentMail,
            VerificationCode = "wrong-code"
        };

        _clubRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(club);

        _codeServiceMock
            .Setup(x => x.ValidateCode(command.Email, command.VerificationCode, VerificationType.ClubRegistration))
            .Returns(false);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.InvalidOrExpiredCode))
            .ReturnsAsync("Invalid code");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid code", result.Message);

        _clubRepositoryMock.Verify(x => x.DeleteAsync(club), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCode_VerifiesClub_ReturnsSuccess()
    {
        // Arrange
        var club = new Club { PresidentMail = "club@mail.com", IsVerified = false };

        var command = new VerificationCommand
        {
            Email = club.PresidentMail,
            VerificationCode = "123456"
        };

        _clubRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(club);

        _codeServiceMock
            .Setup(x => x.ValidateCode(command.Email, command.VerificationCode, VerificationType.ClubRegistration))
            .Returns(true);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.ClubVerifiedSuccess))
            .ReturnsAsync("Verified");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Verified", result.Message);
        Assert.True(club.IsVerified);

        _clubRepositoryMock.Verify(x => x.UpdateAsync(club), Times.Once);
        _codeServiceMock.Verify(
            x => x.RemoveCode(command.Email, VerificationType.ClubRegistration),
            Times.Once
        );
    }
}
