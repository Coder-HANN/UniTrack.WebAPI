using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

namespace UniTrack.Application.Tests.Feature.Event.Query
{
    public class GetClubEventJoinQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserMock;
        private readonly Mock<IEventUserRepository> _eventUserRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationMock;

        public GetClubEventJoinQueryHandlerTests()
        {
            _currentUserMock = new Mock<ICurrentUserServices>();
            _eventUserRepositoryMock = new Mock<IEventUserRepository>();
            _localizationMock = new Mock<ILocalizationService>();

            _localizationMock
                .Setup(x => x.Get(It.IsAny<string>()))
                .ReturnsAsync((string key) => key);
        }

        private GetClubEventJoinQueryHandler CreateHandler()
            => new GetClubEventJoinQueryHandler(
                _currentUserMock.Object,
                _eventUserRepositoryMock.Object,
                _localizationMock.Object);

        [Fact]
        public async Task Handle_ClubIdNull_ShouldReturnUnauthorized()
        {
            // Arrange
            _currentUserMock
                .Setup(x => x.CurrentClub())
                .Returns((Guid?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            _eventUserRepositoryMock.Verify(
                x => x.GetClubEventJoinsByClubIdAsync(It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_RoleUser_ShouldReturnUnauthorized()
        {
            // Arrange
            _currentUserMock
                .Setup(x => x.CurrentClub())
                .Returns(Guid.NewGuid());
            _currentUserMock
                .Setup(x => x.Role())
                .Returns(Role.User);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            _eventUserRepositoryMock.Verify(
                x => x.GetClubEventJoinsByClubIdAsync(It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_RoleNull_ShouldReturnUnauthorized()
        {
            // Arrange
            _currentUserMock
                .Setup(x => x.CurrentClub())
                .Returns(Guid.NewGuid());
            _currentUserMock
                .Setup(x => x.Role())
                .Returns((Role?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            _eventUserRepositoryMock.Verify(
                x => x.GetClubEventJoinsByClubIdAsync(It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_NoEventJoins_ShouldReturnSuccessWithNullData()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _currentUserMock
                .Setup(x => x.CurrentClub())
                .Returns(Guid.NewGuid());
            _currentUserMock
                .Setup(x => x.Role())
                .Returns(Role.Club);

            _eventUserRepositoryMock
                .Setup(x => x.GetClubEventJoinsByClubIdAsync(eventId))
                .ReturnsAsync((List<EventUser>)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new GetClubEventJoinQuery(eventId), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnUserList()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _currentUserMock
                .Setup(x => x.CurrentClub())
                .Returns(Guid.NewGuid());
            _currentUserMock
                .Setup(x => x.Role())
                .Returns(Role.Club);

            var eventUsers = new List<EventUser>
            {
                new EventUser
                {
                    User = new User
                    {
                        Email = "test@uni.com",
                        UserDetail = new UserDetail
                        {
                            Name = "Ali",
                            Surname = "Veli",
                            ProfileImageUrl = "image.png",
                            Department = new Domain.Entities.Department { Name = "Bilgisayar Mühendisliği" },
                            University = new Domain.Entities.University { Name = "Rumeli Üniversitesi" }
                        }
                    }
                }
            };

            _eventUserRepositoryMock
                .Setup(x => x.GetClubEventJoinsByClubIdAsync(eventId))
                .ReturnsAsync(eventUsers);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new GetClubEventJoinQuery(eventId), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().BeNull();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);

            var dto = result.Data.Single();
            dto.Name.Should().Be("Ali");
            dto.Surname.Should().Be("Veli");
            dto.Department.Should().Be("Bilgisayar Mühendisliği");
            dto.UniversityName.Should().Be("Rumeli Üniversitesi");
            dto.Email.Should().Be("test@uni.com");
            dto.ProfileImageUrl.Should().Be("image.png");
        }
    }
}