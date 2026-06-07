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
    private readonly Mock<IUserRepository> _userRepository; // 💡 Yeni eklenen mock

    private readonly ClubRegisterCommandHandler _handler;

    public ClubRegisterCommandHandlerTests()
    {
        _clubRepository = new Mock<IClubRepository>();
        _passwordHasher = new Mock<IPasswordHasher<Club>>();
        _codeService = new Mock<IVerificationCodeService>();
        _transactionService = new Mock<ITransactionService>();
        _localizationService = new Mock<ILocalizationService>();
        _userRepository = new Mock<IUserRepository>(); // 💡 Yeni initialize edildi

        _handler = new ClubRegisterCommandHandler(
            _clubRepository.Object,
            _passwordHasher.Object,
            _codeService.Object,
            _transactionService.Object,
            _localizationService.Object,
            _userRepository.Object); // 💡 Constructor'a parametre olarak geçildi
    }

    private ClubRegisterCommand CreateValidCommand()
    {
        return new ClubRegisterCommand
        {
            ClubName = "Test Club",
            PresidentEmail = "president@test.com",
            ContactEmail = "contact@test.com",
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

        _userRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    // -------------------------
    // ❌ Email zaten bir kullanıcı (User) tarafından kullanılıyor
    // -------------------------
    [Fact]
    public async Task Handle_Should_Return_Fail_When_Email_Already_Used_By_User()
    {
        // Arrange
        var command = CreateValidCommand();

        _userRepository
            .Setup(x => x.GetByEmailAsync(command.ContactEmail))
            .ReturnsAsync(new User()); // Kullanıcı bulundu senaryosu

        _localizationService
            .Setup(x => x.Get(ValidationKeys.EmailAlreadyUsed))
            .ReturnsAsync("Email already used");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Email already used", result.Message);

        _transactionService.Verify(x => x.Rollback(), Times.Once);
        _clubRepository.Verify(x => x.AddAsync(It.IsAny<Club>()), Times.Never);
    }

    // -------------------------
    // ✅ Başarılı yeni kulüp kaydı
    // -------------------------
    [Fact]
    public async Task Handle_Should_Register_Club_Successfully()
    {
        // Arrange
        var command = CreateValidCommand();

        _userRepository
            .Setup(x => x.GetByEmailAsync(command.ContactEmail))
            .ReturnsAsync((User)null);

        _clubRepository
            .Setup(x => x.GetByEmailAsync(command.ContactEmail))
            .ReturnsAsync((Club)null); // Yeni kayıt senaryosu

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
                command.ContactEmail,
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

        _userRepository
            .Setup(x => x.GetByEmailAsync(command.ContactEmail))
            .ReturnsAsync((User)null);

        _clubRepository
            .Setup(x => x.GetByEmailAsync(command.ContactEmail))
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