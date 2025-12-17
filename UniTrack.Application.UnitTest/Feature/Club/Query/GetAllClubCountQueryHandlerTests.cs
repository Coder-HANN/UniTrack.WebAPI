using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Feature.Club.Query;
using Xunit;

public class GetAllClubCountQueryHandlerTests
{
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly GetAllClubCountQueryHandler _handler;

    public GetAllClubCountQueryHandlerTests()
    {
        _clubRepository = new Mock<IClubRepository>();
        _handler = new GetAllClubCountQueryHandler(_clubRepository.Object);
    }

    // -----------------------------
    // ❌ Count = 0 → Fail
    // -----------------------------
    [Fact]
    public async Task Handle_CountIsZero_ReturnsFail()
    {
        // Arrange
        _clubRepository
            .Setup(x => x.GetClubCountAsync())
            .ReturnsAsync(0);

        var query = new GetAllClubCountQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0);
    }

    // -----------------------------
    // ✅ Count > 0 → Success
    // -----------------------------
    [Fact]
    public async Task Handle_CountGreaterThanZero_ReturnsSuccess()
    {
        // Arrange
        _clubRepository
            .Setup(x => x.GetClubCountAsync())
            .ReturnsAsync(15);

        var query = new GetAllClubCountQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(15);
    }
}
