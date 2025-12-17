using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Feature.City.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class CreateCityCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<ICityRepository> _cityRepository;

    private readonly CreateCityCommandHandler _handler;

    public CreateCityCommandHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _cityRepository = new Mock<ICityRepository>();

        _handler = new CreateCityCommandHandler(
            _currentUserServices.Object,
            _cityRepository.Object);
    }

    // --------------------------------------------------
    // ❌ CURRENT USER NULL → UNAUTHORIZED
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Fail_When_CurrentUser_Is_Null()
    {
        // Arrange
        var command = new CreateCityCommand
        {
            Name = "İstanbul"
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Unauthorized", result.Message);

        _cityRepository.Verify(x => x.AddAsync(It.IsAny<City>()), Times.Never);
    }

    // --------------------------------------------------
    // ❌ ROLE USER / CLUB → YETKİSİZ
    // --------------------------------------------------
    [Theory]
    [InlineData(Role.User)]
    [InlineData(Role.Club)]
    public async Task Handle_Should_Fail_When_Role_Is_Not_Admin(Role role)
    {
        // Arrange
        var command = new CreateCityCommand
        {
            Name = "Ankara"
        };

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
        Assert.Equal("Yetkisiz kullanıcı", result.Message);

        _cityRepository.Verify(x => x.AddAsync(It.IsAny<City>()), Times.Never);
    }

    // --------------------------------------------------
    // ✅ ADMIN → CITY CREATED
    // --------------------------------------------------
    [Fact]
    public async Task Handle_Should_Create_City_When_Admin()
    {
        // Arrange
        var command = new CreateCityCommand
        {
            Name = "İzmir"
        };

        _currentUserServices
            .Setup(x => x.CurrentUser())
            .Returns(Guid.NewGuid());

        _currentUserServices
            .Setup(x => x.Role())
            .Returns(Role.Admin);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("City created successfully", result.Message);

        _cityRepository.Verify(x => x.AddAsync(It.Is<City>(c =>
            c.Name == command.Name
        )), Times.Once);
    }
}
