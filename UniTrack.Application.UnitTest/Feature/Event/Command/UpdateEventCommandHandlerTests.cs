using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Domain.Enums;
using Xunit;
namespace UniTrack.Application.Tests.Feature.Event.Command
{
    public class UpdateEventCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServicesMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly UpdateEventCommandHandler _handler;

        public UpdateEventCommandHandlerTests()
        {
            _currentUserServicesMock = new Mock<ICurrentUserServices>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new UpdateEventCommandHandler(
                _currentUserServicesMock.Object,
                _eventRepositoryMock.Object,
                _notificationServiceMock.Object,
                _localizationServiceMock.Object
            );
        }
        [Fact]
        public async Task Handle_Should_Update_Event_Successfully()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var clubId = Guid.NewGuid();

            var existingEvent = new Domain.Entities.Event
            {
                Id = eventId,
                ClubId = clubId,
                Title = "Old Title",
                Description = "Old Desc",
                Status = Status.Private,
            };

            _currentUserServicesMock.Setup(x => x.CurrentClub())
                .Returns(clubId);

            _currentUserServicesMock.Setup(x => x.Role())
                .Returns(Role.Admin);

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync(existingEvent);

            _localizationServiceMock.Setup(x =>
                    x.Get(It.IsAny<string>(), It.IsAny<object[]>()))
                .ReturnsAsync("localized-message");

            var command = new UpdateEventCommand
            {
                Id = eventId,
                Title = "New Title",
                Description = "New Desc",
                Status = Status.Public
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _eventRepositoryMock.Verify(
                x => x.UpdateAsync(It.Is<Domain.Entities.Event>(e =>
                    e.Title == "New Title" &&
                    e.Description == "New Desc" &&
                    e.Status == Status.Public
                )),
                Times.Once);

            _notificationServiceMock.Verify(
                x => x.ClubIsUpdateEventAsync(
                    clubId,
                    It.IsAny<string>()),
                Times.Once);
        }
    }
}