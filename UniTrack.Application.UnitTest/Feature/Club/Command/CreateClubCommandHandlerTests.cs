using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class CreateClubCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ILocalizationService> _localizationService;

    private readonly CreateClubCommandHandler _handler;

    public CreateClubCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _clubRepository = new Mock<IClubRepository>();
        _mapper = new Mock<IMapper>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new CreateClubCommandHandler(
            _currentUserServices.Object,
            _clubRepository.Object,
            _mapper.Object,
            _localizationService.Object);
    }

    // --------------------------------------------------
    // ❌ USER & CLUB NULL → NOT AUTHORIZED
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_User_And_Club_Are_Null()
    {
        // Arrange
        var command = CreateValidCommand();

        _currentUserServices.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _currentUserServices.Setup(x => x.CurrentClub()).Returns((Guid?)null);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not authorized");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Not authorized", result.Message);

        _clubRepository.Verify(x => x.AddAsync(It.IsAny<Club>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ ROLE USER → NOT AUTHORIZED
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_Role_Is_User()
    {
        // Arrange
        var command = CreateValidCommand();

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServices.Setup(x => x.CurrentClub()).Returns((Guid?)null);
        _currentUserServices.Setup(x => x.Role()).Returns(Role.User);

        _localizationService
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Not authorized");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Not authorized", result.Message);

        _clubRepository.Verify(x => x.AddAsync(It.IsAny<Club>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ ADMIN / CLUB → CLUB CREATED
    // --------------------------------------------------
    [Theory]
    [InlineData(Role.Admin)]
    [InlineData(Role.Club)]
    public async Task Handle_Should_Create_Club_When_Authorized(Role role)
    {
        // Arrange
        var command = CreateValidCommand();

        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServices.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserServices.Setup(x => x.Role()).Returns(role);

        _mapper
            .Setup(x => x.Map<Club>(command))
            .Returns(new Club { Name = command.Name });

        _localizationService
            .Setup(x => x.Get(ValidationKeys.ClubCreatedSuccess))
            .ReturnsAsync("Club created successfully");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Club created successfully", result.Message);

        _clubRepository.Verify(x => x.AddAsync(It.Is<Club>(c =>
            c.Name == command.Name
        )), Times.Once);
    }

    // --------------------------------------------------
    // 🔧 HELPER
    // --------------------------------------------------
    private static CreateClubCommand CreateValidCommand()
    {
        return new CreateClubCommand
        {
            Name = "Yazılım Kulübü",
            UniversityId = Guid.NewGuid(),
            President = "Ali Veli",
            ContectEmail = "club@mail.com",
            Description = "Test Club",
            ClubCreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Tag = Tag.Astronomi
        };
    }
}
