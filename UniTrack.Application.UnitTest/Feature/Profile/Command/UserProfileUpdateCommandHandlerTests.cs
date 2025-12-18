using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Profile.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class UserProfileUpdateCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUser;
    private readonly Mock<IUserDetailRepository> _userDetailRepo;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<ILocalizationService> _localization;

    private readonly UserProfileUpdateCommandHandler _handler;

    public UserProfileUpdateCommandHandlerTests()
    {
        _currentUser = new Mock<ICurrentUserServices>();
        _userDetailRepo = new Mock<IUserDetailRepository>();
        _userRepo = new Mock<IUserRepository>();
        _localization = new Mock<ILocalizationService>();

        _handler = new UserProfileUpdateCommandHandler(
            _currentUser.Object,
            _userDetailRepo.Object,
            _userRepo.Object,
            _localization.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUser.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _localization.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                     .ReturnsAsync("Unauthorized");

        var result = await _handler.Handle(new UserProfileUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_RoleIsClub_ReturnsUnauthorized()
    {
        _currentUser.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUser.Setup(x => x.Role()).Returns(Role.Club);
        _localization.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                     .ReturnsAsync("Unauthorized");

        var result = await _handler.Handle(new UserProfileUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ReturnsError()
    {
        var userId = Guid.NewGuid();

        _currentUser.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUser.Setup(x => x.Role()).Returns(Role.User);

        _userDetailRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserDetail, bool>>>()))
            .ReturnsAsync(new UserDetail { UserId = userId });

        _userRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new User { Id = userId });

        _userRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<User>
            {
                new User { Email = "test@mail.com" }
            });

        _localization.Setup(x => x.Get(ValidationKeys.UserEmailAlreadyExists))
                     .ReturnsAsync("Mail exists");

        var command = new UserProfileUpdateCommand
        {
            Email = "test@mail.com"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Mail exists");
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesProfileSuccessfully()
    {
        var userId = Guid.NewGuid();

        _currentUser.Setup(x => x.CurrentUser()).Returns(userId);
        _currentUser.Setup(x => x.Role()).Returns(Role.User);

        var userDetail = new UserDetail
        {
            UserId = userId,
            Name = "Old"
        };

        var user = new User
        {
            Id = userId,
            Email = "old@mail.com"
        };

        _userDetailRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserDetail, bool>>>()))
            .ReturnsAsync(userDetail);

        _userRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

        _userRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<User>());

        _localization.Setup(x => x.Get(ValidationKeys.ProfileUpdatedSuccessfully))
                     .ReturnsAsync("Updated");

        var command = new UserProfileUpdateCommand
        {
            Name = "NewName"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        _userDetailRepo.Verify(x => x.UpdateAsync(It.IsAny<UserDetail>()), Times.Once);
        _userRepo.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Updated");
    }
}
