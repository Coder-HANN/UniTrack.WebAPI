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
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;
using UniTrack.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using UniTrack.Application.DTOs.Event;

public class CreateEventCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserServiceMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly Mock<IGoogleSheetCreationService> _sheetServiceMock = new();
    private readonly Mock<IQrCodeService> _qrCodeServiceMock = new();
    private readonly Mock<IStorageService> _storageServiceMock = new();
    private readonly Mock<ILocalizationService> _localizationServiceMock = new();
    private readonly Mock<ITransactionService> _transactionServiceMock = new();
    private readonly Mock<IValidator<CreateEventCommand>> _validatorMock = new();

    private CreateEventCommandHandler CreateHandler()
        => new(
            _currentUserServiceMock.Object,
            _eventRepositoryMock.Object,
            _mapperMock.Object,
            _notificationServiceMock.Object,
            _sheetServiceMock.Object,
            _qrCodeServiceMock.Object,
            _storageServiceMock.Object,
            _localizationServiceMock.Object,
            _transactionServiceMock.Object,
            _validatorMock.Object
        );

    [Fact]
    public async Task Handle_ValidClubRequest_ShouldCreateEventSuccessfully()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        var clubName = "Computer Club";
        var command = new CreateEventCommand { Title = "Tech Summit", EventTag = EventTag.Social };

        // 1. Kimlik ve Yetki Mocking
        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.Club);

        // 2. Validation Mocking (Başarılı geçmesi için)
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // 3. Mapping ve Repository Mocking
        var domainEvent = new UniTrack.Domain.Entities.Event { Id = Guid.NewGuid(), Title = "Tech Summit" };
        _mapperMock.Setup(x => x.Map<UniTrack.Domain.Entities.Event>(command)).Returns(domainEvent);


        _eventRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UniTrack.Domain.Entities.Event>())).ReturnsAsync(domainEvent);

        // 4. Dış Servisler (QR, Storage, Sheets)
        _qrCodeServiceMock.Setup(x => x.GenerateQrCodeAsync(It.IsAny<Guid>())).ReturnsAsync(new byte[] { 1 });
        _storageServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), null)).ReturnsAsync("https://qr-url");
        _sheetServiceMock.Setup(x => x.CreateSheetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("sheet-123");

        // 5. Localization
        _localizationServiceMock.Setup(x => x.Get(ValidationKeys.EventCreatedSuccess)).ReturnsAsync("Başarılı");

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _transactionServiceMock.Verify(x => x.Begin(), Times.Once);
        _transactionServiceMock.Verify(x => x.Commit(), Times.Once);
        _eventRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<UniTrack.Domain.Entities.Event>()), Times.Once);
        _notificationServiceMock.Verify(x => x.ClubIsCreateEventAsync(clubId, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFail()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.Club);
        _currentUserServiceMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());

        var failures = new List<ValidationFailure> { new("Title", "Başlık zorunlu") };
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateEventCommand>(), default))
            .ReturnsAsync(new ValidationResult(failures));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new CreateEventCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Başlık zorunlu");
        _transactionServiceMock.Verify(x => x.Begin(), Times.Never);
    }
}