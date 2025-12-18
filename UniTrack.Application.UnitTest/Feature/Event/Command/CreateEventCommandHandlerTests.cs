using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Abstraction.Services.QrCode;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Tests.Feature.Event.Command
{
    public class CreateEventCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IGoogleSheetCreationService> _sheetServiceMock;
        private readonly Mock<IQrCodeService> _qrCodeServiceMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly CreateEventCommandHandler _handler;

        public CreateEventCommandHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserServices>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _mapperMock = new Mock<IMapper>();
            _notificationServiceMock = new Mock<INotificationService>();
            _sheetServiceMock = new Mock<IGoogleSheetCreationService>();
            _qrCodeServiceMock = new Mock<IQrCodeService>();
            _storageServiceMock = new Mock<IStorageService>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new CreateEventCommandHandler(
                _currentUserServiceMock.Object,
                _eventRepositoryMock.Object,
                _mapperMock.Object,
                _notificationServiceMock.Object,
                _sheetServiceMock.Object,
                _qrCodeServiceMock.Object,
                _storageServiceMock.Object,
                _localizationServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserNotAuthorized_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);
            _currentUserServiceMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);
            _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Yetkisiz");

            var command = new CreateEventCommand { Title = "Test Event" };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Yetkisiz");

            _eventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Event>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidClubRequest_ShouldCreateEventSuccessfully()
        {
            // Arrange
            var clubId = Guid.NewGuid();

            _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserServiceMock.Setup(x => x.CurrentClub()).Returns(clubId);
            _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.Club);

            var domainEvent = new Domain.Entities.Event
            {
                Id = Guid.NewGuid(),
                Title = "Tech Summit",
                Club = new Club { Name = "Computer Club" }
            };

            _mapperMock.Setup(x =>
                x.Map<Domain.Entities.Event>(It.IsAny<CreateEventCommand>()))
                .Returns(domainEvent);

            _eventRepositoryMock.Setup(x =>
                x.AddAsync(It.IsAny<Domain.Entities.Event>()))
                .ReturnsAsync(domainEvent);

            _qrCodeServiceMock.Setup(x =>
                x.GenerateQrCodeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new byte[] { 1, 2, 3 });

            _storageServiceMock.Setup(x =>
                x.UploadFileAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("https://qr-url");


            _sheetServiceMock.Setup(x =>
                x.CreateSheetAsync(It.IsAny<string>()))
                .ReturnsAsync("sheet-id");

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.EventCreatedNotification,
                      domainEvent.Title,
                      domainEvent.Club.Name))
                .ReturnsAsync("Bildirim");

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.EventCreatedSuccess))
                .ReturnsAsync("Etkinlik oluşturuldu");

            var command = new CreateEventCommand
            {
                Title = "Tech Summit",
                Tag = Tag.Bilim,
                Status = Status.Public
            };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Etkinlik oluşturuldu");

            _eventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Event>()), Times.Once);
            _eventRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Event>()), Times.Once);

            _qrCodeServiceMock.Verify(x => x.GenerateQrCodeAsync(It.IsAny<Guid>()), Times.Once);
            _storageServiceMock.Verify(x =>x.UploadFileAsync(It.IsAny<byte[]>(),It.IsAny<string>(),It.IsAny<string>()),Times.Once);

            _sheetServiceMock.Verify(x => x.CreateSheetAsync("Tech Summit"), Times.Once);

            _notificationServiceMock.Verify(x =>
                x.ClubIsCreateEventAsync(clubId, "Bildirim"),
                Times.Once);
        }
    }
}
