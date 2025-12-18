using System;
using System.Collections.Generic;
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

public class ClubProfileUpdateCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUser;
    private readonly Mock<IClubRepository> _clubRepo;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<ILocalizationService> _localization;

    private readonly ClubProfileUpdateCommandHandler _handler;

    public ClubProfileUpdateCommandHandlerTests()
    {
        _currentUser = new Mock<ICurrentUserServices>();
        _clubRepo = new Mock<IClubRepository>();
        _userRepo = new Mock<IUserRepository>();
        _localization = new Mock<ILocalizationService>();

        _handler = new ClubProfileUpdateCommandHandler(
            _currentUser.Object,
            _clubRepo.Object,
            _userRepo.Object,
            _localization.Object);
    }

    [Fact]
    public async Task Handle_NotAuthorized_ReturnsError()
    {
        _currentUser.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _localization.Setup(x => x.Get(ValidationKeys.NotAuthorized))
                     .ReturnsAsync("Unauthorized");

        var result = await _handler.Handle(new ClubProfileUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ClubNotFound_ReturnsError()
    {
        var clubId = Guid.NewGuid();

        _currentUser.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUser.Setup(x => x.Role()).Returns(Role.Club);

        _clubRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>()))
                 .ReturnsAsync((Club)null);

        _localization.Setup(x => x.Get(ValidationKeys.ClubNotFound))
                     .ReturnsAsync("Club not found");

        var result = await _handler.Handle(new ClubProfileUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Club not found");
    }

    [Fact]
    public async Task Handle_EmailAlreadyUsed_ReturnsError()
    {
        var clubId = Guid.NewGuid();

        _currentUser.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUser.Setup(x => x.Role()).Returns(Role.Club);

        _clubRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>()))
            .ReturnsAsync(new Club { Id = clubId });

        _clubRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Club>
            {
                new Club { PresidentMail = "test@mail.com" }
            });

        _userRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<User>());

        _localization.Setup(x => x.Get(ValidationKeys.EmailAlreadyUsed))
                     .ReturnsAsync("Mail already used");

        var command = new ClubProfileUpdateCommand
        {
            PresidentMail = "test@mail.com"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Mail already used");
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesClubSuccessfully()
    {
        var clubId = Guid.NewGuid();

        _currentUser.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUser.Setup(x => x.Role()).Returns(Role.Club);

        _clubRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Club, bool>>>()))
            .ReturnsAsync(new Club { Id = clubId });

        _clubRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Club>());

        _userRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<User>());

        _localization.Setup(x => x.Get(ValidationKeys.ProfileUpdatedSuccessfully))
                     .ReturnsAsync("Updated");

        var command = new ClubProfileUpdateCommand
        {
            Name = "New Club Name"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        _clubRepo.Verify(x => x.UpdateAsync(It.IsAny<Club>()), Times.Once);
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Updated");
    }
}
