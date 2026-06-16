using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Ban.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class BanForUserCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IBanRepository> _banRepository;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly BanForUserCommandHandler _handler;

    // Localization key → human-readable value mapping used in assertions.
    // These must match whatever ValidationKeys.* constants resolve to.
    private const string NotAuthorizedMessage = "Yetkisiz kullanıcı";
    private const string UserNotFoundMessage = "Kullanıcı bulunamadı";
    private const string OperationSuccessMessage = "İşlem başarılı";

    public BanForUserCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _userRepository = new Mock<IUserRepository>();
        _banRepository = new Mock<IBanRepository>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new BanForUserCommandHandler(
            _currentUserServices.Object,
            _userRepository.Object,
            _banRepository.Object,
            _localizationService.Object);

        // Global localization setup — every key returns a known human-readable string
        // so assertions can target real messages, not raw key constants.
        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync(NotAuthorizedMessage);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.UserNotFound))
            .ReturnsAsync(UserNotFoundMessage);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.OperationSuccessful))
            .ReturnsAsync(OperationSuccessMessage);
    }

    // --------------------------------------------------
    // ❌ CURRENT USER NULL → UNAUTHORIZED
    // --------------------------------------------------
    // The handler has an EXPLICIT null guard on adminId BEFORE the role check.
    // This is a separate branch from the role check — both return NotAuthorized.
    [Fact]
    public async Task Handle_Should_Fail_When_CurrentUser_Is_Null()
    {
        // Arrange
        var command = new BanForUserCommand { UserId = Guid.NewGuid() };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(NotAuthorizedMessage, result.Message);

        // Neither user lookup nor ban insert should be reached
        _userRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ ROLE USER / CLUB → UNAUTHORIZED
    // --------------------------------------------------
    // The handler checks role AFTER the null-user guard.
    // Role.User and Role.Club both hit the unauthorized branch.
    [Theory]
    [InlineData(Role.User)]
    [InlineData(Role.Club)]
    public async Task Handle_Should_Fail_When_Role_Is_Not_Admin(Role role)
    {
        // Arrange
        var command = new BanForUserCommand { UserId = Guid.NewGuid() };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(role);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(NotAuthorizedMessage, result.Message);

        _userRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ USER NOT FOUND
    // --------------------------------------------------
    // After the auth checks pass, the handler calls userRepository.GetByIdAsync(request.UserId).
    // Returning null triggers the UserNotFound branch.
    [Fact]
    public async Task Handle_Should_Fail_When_User_Not_Found()
    {
        // Arrange
        var command = new BanForUserCommand { UserId = Guid.NewGuid() };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _userRepository
            .Setup(x => x.GetByIdAsync(command.UserId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(UserNotFoundMessage, result.Message);

        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ USER FROM DIFFERENT UNIVERSITY → UNAUTHORIZED
    // --------------------------------------------------
    // The handler checks user.UserDetail.UniverstiyId != currentUserServices.UniversityId().
    // If they differ the request is rejected even for an Admin.
    [Fact]
    public async Task Handle_Should_Fail_When_User_Belongs_To_Different_University()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var adminUniversityId = Guid.NewGuid();
        var otherUniversityId = Guid.NewGuid(); // intentionally different

        var command = new BanForUserCommand
        {
            UserId = userId,
            LastDate = DateTime.UtcNow.AddDays(3),
            Description = "Test"
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        _currentUserServices
            .Setup(x => x.UniversityId())
            .Returns(adminUniversityId);

        _userRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User
            {
                Id = userId,
                UserDetail = new UserDetail { UniverstiyId = otherUniversityId }
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(NotAuthorizedMessage, result.Message);

        _banRepository.Verify(x => x.AddAsync(It.IsAny<Ban>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ ADMIN & USER IN SAME UNIVERSITY → BAN SUCCESSFUL
    // --------------------------------------------------
    // All guards pass: adminId non-null, role Admin,
    // user exists, and UniverstiyId matches.
    // The handler creates a Ban with Role.User and IsBanned = true.
    [Fact]
    public async Task Handle_Should_Ban_User_Successfully_When_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var universityId = Guid.NewGuid();
        var lastDate = DateTime.UtcNow.AddDays(3);
        const string desc = "Topluluk kurallarına aykırı davranış";

        var command = new BanForUserCommand
        {
            UserId = userId,
            LastDate = lastDate,
            Description = desc
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        // UniversityId() must be set up BEFORE the handler calls it
        // at the university-match guard (line 64 in handler).
        _currentUserServices
            .Setup(x => x.UniversityId())
            .Returns(universityId);

        _userRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User
            {
                Id = userId,
                UserDetail = new UserDetail
                {
                    // Must equal UniversityId() above — otherwise handler rejects the request
                    UniverstiyId = universityId
                }
            });

        _banRepository
            .Setup(x => x.AddAsync(It.IsAny<Ban>()))
            .ReturnsAsync((Ban ban) => ban);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — response
        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OperationSuccessMessage, result.Message);

        // Assert — Ban entity written to repository with correct fields
        _banRepository.Verify(x => x.AddAsync(It.Is<Ban>(b =>
            b.UserId == userId &&
            b.Role == Role.User &&
            b.IsBanned == true &&
            b.LastDate == lastDate &&
            b.Description == desc
        )), Times.Once);
    }
}