using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Club;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Domain.Entities;
using Xunit;

public class GetAllClubQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServices;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<IBaseEntityRepository<Club>> _baseEntityRepository;
    private readonly Mock<ILocalizationService> _localizationService;
    private readonly GetAllClubQueryHandler _handler;

    public GetAllClubQueryHandlerTests()
    {
        _currentUserServices = new Mock<ICurrentUserServices>();
        _clubRepository = new Mock<IClubRepository>();
        _baseEntityRepository = new Mock<IBaseEntityRepository<Club>>();
        _localizationService = new Mock<ILocalizationService>();

        _handler = new GetAllClubQueryHandler(
            _currentUserServices.Object,
            _clubRepository.Object,
            _baseEntityRepository.Object,
            _localizationService.Object);
    }

// TO DO: Bakılacak

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPagedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServices.Setup(x => x.CurrentUser()).Returns(userId);

        // Handler içindeki Select dönüşümünde .Name property'lerine erişildiği için 
        // ilişkili tabloların (University, City) mock veride null olmaması gerekir.
        var clubs = new List<Club>
        {
            new Club
            {
                Id = Guid.NewGuid(),
                Name = "Test Club",
                Follower = 10,
                President = "President",
                ContectEmail = "test@mail.com",
                University = new University { Name = "Rumeli University" },
                City = new City { Name = "Istanbul" },
                UserClubs = new List<UserClub>()
            }
        };

        _clubRepository
            .Setup(x => x.GetAllClubListAsync())
            .ReturnsAsync(clubs);

        var pagingResult = new Mock<IPagingExecutionResult<GetAllClubQueryResponseDTO>>().Object;

        // Moq'un IEnumerable tipindeki koleksiyon eşleşmesini doğru yakalayabilmesi için It.IsAny<IEnumerable<...>>() kullanıldı.
        _baseEntityRepository
            .Setup(x => x.GetPagedResult(
                It.IsAny<IEnumerable<GetAllClubQueryResponseDTO>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Func<IQueryable<GetAllClubQueryResponseDTO>, IOrderedQueryable<GetAllClubQueryResponseDTO>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResult);

        var query = new GetAllClubQuery { Page = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().BeNull();
    }
}