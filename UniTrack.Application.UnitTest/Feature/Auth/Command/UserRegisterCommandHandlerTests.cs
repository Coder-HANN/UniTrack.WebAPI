using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.UserHub;
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

    private readonly UserRegisterCommandHandler _handler;

    public UserRegisterCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _userDetailRepository = new Mock<IUserDetailRepository>();
        _passwordHasher = new Mock<IPasswordHasher<User>>();
        _countService = new Mock<IUserRegisterCountService>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new UserRegisterCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _userDetailRepository.Object,
            _countService.Object,
            _localizationService.Object);
    }

    private UserRegisterCommand CreateValidCommand()
    {
        return new UserRegisterCommand
        {
            Name = "Ali",
            Surname = "Veli",
            Email = "user@test.com",
            Password = "123456",
            DepartmentId = 1,
            UniversityId = Guid.NewGuid(),
            CityId = 34,
            Gender = Gender.Male,
            BirthDate = new DateOnly(2002, 5, 10),
            PhoneNumber = 5554443322,
            Graduaiton_Date = DateTime.UtcNow.AddYears(2)
        };
    }

    // --------------------------------------------------
    // ❌ EMAIL ZATEN VAR
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Email_Already_Exists()
    {
        // Arrange
        var command = CreateValidCommand();

        _userRepository
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(new User());

        _localizationService
            .Setup(x => x.Get(ValidationKeys.UserEmailAlreadyExists))
            .ReturnsAsync("User email already exists");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User email already exists", result.Message);

        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        _userDetailRepository.Verify(x => x.AddAsync(It.IsAny<UserDetail>()), Times.Never);
        _countService.Verify(x => x.NotifyUserCountUpdatedAsync(), Times.Never);
    }

    // --------------------------------------------------
    // ✅ BAŞARILI REGISTER
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Register_User_Successfully()
    {
        // Arrange
        var command = CreateValidCommand();

        _userRepository
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync((User)null);

        _passwordHasher
            .Setup(x => x.HashPassword(null, command.Password))
            .Returns("hashed_password");

        _localizationService
            .Setup(x => x.Get(ValidationKeys.UserRegisterSuccess))
            .ReturnsAsync("User registered successfully");

        _countService
            .Setup(x => x.NotifyUserCountUpdatedAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("User registered successfully", result.Message);

        _userRepository.Verify(x => x.AddAsync(It.Is<User>(u =>
            u.Email == command.Email &&
            u.Password == "hashed_password" &&
            u.Role == Role.User
        )), Times.Once);

        _userDetailRepository.Verify(x => x.AddAsync(It.Is<UserDetail>(d =>
            d.Name == command.Name &&
            d.Surname == command.Surname &&
            d.UniverstiyId == command.UniversityId &&
            d.DepartmentId == command.DepartmentId &&
            d.CityId == command.CityId &&
            d.Gender == command.Gender &&
            d.BirthDate == command.BirthDate &&
            d.PhoneNumber == command.PhoneNumber &&
            d.Graduaiton_Date == command.Graduaiton_Date
        )), Times.Once);

        _countService.Verify(x => x.NotifyUserCountUpdatedAsync(), Times.Once);
    }
}
