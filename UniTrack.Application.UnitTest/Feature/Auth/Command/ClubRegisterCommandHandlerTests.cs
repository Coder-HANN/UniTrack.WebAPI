using Microsoft.AspNetCore.Identity;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Auth.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class ClubRegisterCommandHandlerTests
{
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<IPasswordHasher<Club>> _passwordHasher;
    private readonly Mock<IVerificationCodeService> _codeService;
    private readonly Mock<ITransactionService> _transactionService;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly ClubRegisterCommandHandler _handler;

    public ClubRegisterCommandHandlerTests()
    {
        _clubRepository = new Mock<IClubRepository>();
        _passwordHasher = new Mock<IPasswordHasher<Club>>();
        _codeService = new Mock<IVerificationCodeService>();
        _transactionService = new Mock<ITransactionService>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new ClubRegisterCommandHandler(
            _clubRepository.Object,
            _passwordHasher.Object,
            _codeService.Object,
            _transactionService.Object,
            _localizationService.Object);
    }

    private ClubRegisterCommand CreateValidCommand()
    {
        return new ClubRegisterCommand
        {
            ClubName = "Test Club",
            PresidentEmail = "president@test.com",
            ContectEmail = "contact@test.com",
            Password = "123456",
            PresidentName = "John Doe",
            UniversityId = Guid.NewGuid(),
            CityId = 34,
            Tag = Tag.Bilim
        };
    }

    // -------------------------
    // ❌ PresidentEmail boş
    // -------------------------
    [Fact]
    public async Task Handle_Should_Return_Fail_When_PresidentEmail_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.PresidentEmail = "";

        _localizationService
            .Setup(x => x.Get(ValidationKeys.PresidentEmailRequired))
            .ReturnsAsync("President email is required");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("President email is required", result.Message);

        _clubRepository.Verify(x => x.GetByEmailAndVerifyAsync(It.IsAny<string>()), Times.Never);
    }

    // -------------------------
    // ❌ Email zaten kayıtlı
    // -------------------------
    [Fact]
    public async Task Handle_Should_Return_Fail_When_Club_Email_Already_Exists()
    {
        // Arrange
        var command = CreateValidCommand();

        _clubRepository
            .Setup(x => x.GetByEmailAndVerifyAsync(command.PresidentEmail))
            .ReturnsAsync(new Club());

        _localizationService
            .Setup(x => x.Get(ValidationKeys.ClubEmailAlreadyExists))
            .ReturnsAsync("Club email already exists");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Club email already exists", result.Message);

        _transactionService.Verify(x => x.Begin(), Times.Never);
        _clubRepository.Verify(x => x.AddAsync(It.IsAny<Club>()), Times.Never);
    }

    // -------------------------
    // ✅ Başarılı kayıt
    // -------------------------

    [Fact]
    public async Task Handle_Should_Register_Club_Successfully()
    {
        // Arrange
        var command = CreateValidCommand();

        _clubRepository
            .Setup(x => x.GetByEmailAndVerifyAsync(command.PresidentEmail))
            .ReturnsAsync((Club)null);

        _passwordHasher
            .Setup(x => x.HashPassword(It.IsAny<Club>(), command.Password))
            .Returns("hashed_password");

        _codeService
            .Setup(x => x.GenerateAndSendCodeAsync(
                It.IsAny<string>(),
                VerificationType.ClubRegistration))
            .Returns(Task.CompletedTask);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.VerificationCodeSent))
            .ReturnsAsync("Verification code sent");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _codeService.Verify(
            x => x.GenerateAndSendCodeAsync(
                command.PresidentEmail,
                VerificationType.ClubRegistration),
            Times.Once);
    }

    // -------------------------
    // ❌ Exception → Rollback
    // -------------------------
    [Fact]
    public async Task Handle_Should_Rollback_When_Exception_Occurs()
    {
        // Arrange
        var command = CreateValidCommand();

        _clubRepository
            .Setup(x => x.GetByEmailAndVerifyAsync(command.PresidentEmail))
            .ReturnsAsync((Club)null);

        _clubRepository
            .Setup(x => x.AddAsync(It.IsAny<Club>()))
            .ThrowsAsync(new Exception("DB Error"));

        _localizationService
            .Setup(x => x.Get(ValidationKeys.ClubRegisterFailed))
            .ReturnsAsync("Club register failed");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Club register failed", result.Message);

        _transactionService.Verify(x => x.Begin(), Times.Once);
        _transactionService.Verify(x => x.Rollback(), Times.Once);
        _transactionService.Verify(x => x.Commit(), Times.Never);
    }
}
