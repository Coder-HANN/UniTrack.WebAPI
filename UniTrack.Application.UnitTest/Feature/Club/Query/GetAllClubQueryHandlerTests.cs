using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.DTOs.Club;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetAllClubQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<BaseEntityRepository<Club>> _baseEntityRepository;

    private readonly GetAllClubQueryHandler _handler;

    public GetAllClubQueryHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _clubRepository = new Mock<IClubRepository>();

        // BaseEntityRepository concrete olduğu için mock böyle
        _baseEntityRepository = new Mock<BaseEntityRepository<Club>>(null);

        _handler = new GetAllClubQueryHandler(
            _currentUserServices.Object,
            _clubRepository.Object,
            _baseEntityRepository.Object);
    }

    // ----------------------------------------------------
    // ❌ Unauthorized
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_UserAndClubNull_ReturnsUnauthorized()
    {
        // Arrange
        _currentUserServices.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _currentUserServices.Setup(x => x.CurrentClub()).Returns((Guid?)null);

        var query = new GetAllClubQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Unauthorized");
    }

    // ----------------------------------------------------
    // ❌ No clubs found
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_ClubListEmpty_ReturnsFail()
    {
        // Arrange
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

        _clubRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Club>());

        var query = new GetAllClubQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("No clubs found");
    }

    // ----------------------------------------------------
    // ✅ Success
    // ----------------------------------------------------
    [Fact]
    public async Task Handle_ValidRequest_ReturnsPagedResult()
    {
        // Arrange
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

        var clubs = new List<Club>
        {
            new Club
            {
                Id = Guid.NewGuid(),
                Name = "Test Club",
                Follower = 10,
                President = "President",
                ContectEmail = "test@mail.com"
            }
        };

        _clubRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(clubs);

        var pagingResult = new Mock<IPagingExecutionResult<GetAllClubQueryResponseDTO>>().Object;

        _baseEntityRepository
            .Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllClubQueryResponseDTO>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Func<IQueryable<GetAllClubQueryResponseDTO>, IOrderedQueryable<GetAllClubQueryResponseDTO>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResult);

        var query = new GetAllClubQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
