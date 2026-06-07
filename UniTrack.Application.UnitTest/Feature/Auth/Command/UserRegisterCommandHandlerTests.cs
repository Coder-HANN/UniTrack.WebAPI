using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Abstraction.Services.UserHub;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Auth.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class UserRegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IUserDetailRepository> _userDetailRepository;
    private readonly Mock<IPasswordHasher<User>> _passwordHasher;
    private readonly Mock<IUserRegisterCountService> _countService;
    private readonly Mock<ILocalizationService> _localizationService;
    private readonly Mock<ITransactionService> _transactionService;
    private readonly Mock<IVerificationCodeService> _verificationCodeService;
    private readonly Mock<IClubRepository> _clubRepository; // 💡 Yeni eklenen mock

    private readonly UserRegisterCommandHandler _handler;

    public UserRegisterCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _userDetailRepository = new Mock<IUserDetailRepository>();
        _passwordHasher = new Mock<IPasswordHasher<User>>();
        _countService = new Mock<IUserRegisterCountService>();
        _localizationService = new Mock<ILocalizationService>();
        _transactionService = new Mock<ITransactionService>();
        _verificationCodeService = new Mock<IVerificationCodeService>();
        _clubRepository = new Mock<IClubRepository>(); // 💡 Yeni initialize edildi

        _handler = new UserRegisterCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _userDetailRepository.Object,
            _countService.Object,
            _transactionService.Object,
            _localizationService.Object,
            _verificationCodeService.Object,
            _clubRepository.Object); // 💡 Son parametre olarak constructor'a geçildi
    }

    private UserRegisterCommand CreateValidCommand() => new()
    {
        Name = "Bedirhan",
        Surname = "Korkmaz",
        Email = "test@uni.com",
        Password = "securePassword123",
        UniversityId = Guid.NewGuid(),
        DepartmentId = 1,
        CityId = 34,
        Gender = Gender.Male,
        BirthDate = new DateOnly(2002, 1, 1),
        Graduaiton_Date = DateTime.UtcNow.AddYears(1)
    };

    // ----------------------------------------------------------------------
    // ❌ Kayıt olmaya çalışan Email zaten bir Kulüp (Club) tarafından alınmışsa
    // ----------------------------------------------------------------------
    [Fact]
    public async Task Handle_EmailAlreadyExistsAsClub_ReturnsFail()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingClub = new Club { ContectEmail = command.Email };

        _clubRepository.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(existingClub);
        _localizationService.Setup(x => x.Get(ValidationKeys.EmailAlreadyUsed)).ReturnsAsync("Email zaten kullanımda");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Email zaten kullanımda");
        _transactionService.Verify(x => x.Rollback(), Times.Once);
        _userRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    // ----------------------------------------------------------------------
    // ❌ Kullanıcı zaten var ve doğrulanmışsa (Verified)
    // ----------------------------------------------------------------------
    [Fact]
    public async Task Handle_UserExistsAndVerified_ReturnsFail()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingUser = new User { Email = command.Email, IsVerified = true };

        _clubRepository.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync((Club)null);
        _userRepository.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(existingUser);
        _localizationService.Setup(x => x.Get(ValidationKeys.UserEmailAlreadyExists)).ReturnsAsync("Email zaten var");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Email zaten var");
        _transactionService.Verify(x => x.Rollback(), Times.Once);
    }

    // ----------------------------------------------------------------------
    // 🔄 Kullanıcı var ama doğrulanmamışsa (Not Verified) -> Güncelleme ve Kod Gönderimi
    // ----------------------------------------------------------------------
    [Fact]
    public async Task Handle_UserExistsButNotVerified_UpdatesAndSendsNewCode()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingUser = new User { Id = Guid.NewGuid(), Email = command.Email, IsVerified = false };
        var existingDetail = new UserDetail { UserId = existingUser.Id };

        _clubRepository.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync((Club)null);
        _userRepository.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(existingUser);
        _userDetailRepository.Setup(x => x.GetByUserIdAsync(existingUser.Id)).ReturnsAsync(existingDetail);
        _passwordHasher.Setup(x => x.HashPassword(null, command.Password)).Returns("new_hashed_password");
        _localizationService.Setup(x => x.Get(ValidationKeys.UserRegisterSuccess)).ReturnsAsync("Başarılı");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepository.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Password == "new_hashed_password")), Times.Once);
        _userDetailRepository.Verify(x => x.UpdateAsync(It.IsAny<UserDetail>()), Times.Once);
        _verificationCodeService.Verify(x => x.GenerateAndSendCodeAsync(command.Email, VerificationType.UserRegistration), Times.Once);
        _transactionService.Verify(x => x.Commit(), Times.Once);
    }

    // ----------------------------------------------------------------------
    // ✅ Tamamen Yeni Kullanıcı Kaydı -> Başarılı
    // ----------------------------------------------------------------------
    [Fact]
    public async Task Handle_NewUserRegistration_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        _clubRepository.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync((Club)null);
        _userRepository.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync((User)null);
        _passwordHasher.Setup(x => x.HashPassword(null, command.Password)).Returns("hashed_password");
        _localizationService.Setup(x => x.Get(ValidationKeys.UserRegisterSuccess)).ReturnsAsync("Kayıt Başarılı");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _userDetailRepository.Verify(x => x.AddAsync(It.IsAny<UserDetail>()), Times.Once);
        _countService.Verify(x => x.NotifyUserCountUpdatedAsync(), Times.Once);
        _verificationCodeService.Verify(x => x.GenerateAndSendCodeAsync(command.Email, VerificationType.UserRegistration), Times.Once);
        _transactionService.Verify(x => x.Commit(), Times.Once);
    }

    // ----------------------------------------------------------------------
    // ❌ Genel Hata Durumu (Exception) -> Rollback
    // ----------------------------------------------------------------------
    [Fact]
    public async Task Handle_GeneralException_RollbacksAndReturnsFail()
    {
        // Arrange
        var command = CreateValidCommand();
        _clubRepository.Setup(x => x.GetByEmailAsync(command.Email)).ThrowsAsync(new Exception("DB Error"));
        _localizationService.Setup(x => x.Get("İşlem başarısız")).ReturnsAsync("Hata oluştu");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Hata oluştu");
        _transactionService.Verify(x => x.Rollback(), Times.Once);
    }
}