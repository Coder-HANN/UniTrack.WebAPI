using AutoMapper;
using FluentValidation;
using MediatR;
using StackExchange.Redis;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Abstraction.Services.QrCode;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Event.Command
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, ServiceResponse<CreateEventResponseDTO>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IEventRepository eventRepository;
        private readonly IMapper mapper;
        private readonly INotificationService notificationProducer;
        private readonly IGoogleSheetCreationService googleSheetCreationService;
        private readonly IQrCodeService qrCodeService;
        private readonly IStorageService storageService;
        private readonly ILocalizationService localization;
        private readonly ITransactionService transactionService;
        private readonly IValidator<CreateEventCommand> _validator;

        public CreateEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            IMapper mapper,
            INotificationService notificationProducer,
            IGoogleSheetCreationService googleSheetCreationService,
            IQrCodeService qrCodeService,
            IStorageService storageService,
            ILocalizationService localizationService,
            ITransactionService transactionService,
            IValidator<CreateEventCommand> validator)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.mapper = mapper;
            this.notificationProducer = notificationProducer;
            this.googleSheetCreationService = googleSheetCreationService;
            this.qrCodeService = qrCodeService;
            this.storageService = storageService;
            this.localization = localizationService;
            this.transactionService = transactionService;
            this._validator = validator;
        }

        public async Task<ServiceResponse<CreateEventResponseDTO>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (userId == null && clubId == null || role == null || role == Domain.Enums.Role.User)
            {
                return ServiceResponse<CreateEventResponseDTO>.Fail(await localization.Get(ValidationKeys.NotAuthorized));
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // Hataları al ve Frontend'e dön
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResponse<CreateEventResponseDTO>.Fail(errors);
            }

            transactionService.Begin();
            try {
                
                    var newEvent = mapper.Map<Domain.Entities.Event>(request);

                    // 2. CHECK-IN TOKEN OLUŞTURMA
                    var checkInToken = Guid.NewGuid();
                    newEvent.CheckInToken = checkInToken;

                    var clubName = await eventRepository.GetClubNameByIdAsync(clubId.Value);
                // 3. ETKİNLİĞİ DB'YE KAYDET (ID'yi almak için)
                var createdEvent = await eventRepository.AddAsync(newEvent);
                    

                    string? qrUrl = null;
                    string? sheetsId = null;

                    if (request.ImageUrls != null && request.ImageUrls.Any())
                    {
                        var images = request.ImageUrls.Select((url, index) => new Domain.Entities.EventImage
                        {
                            EventId = createdEvent.Id,
                            ImageUrl = url,
                            IsCover = index == 0, // ilk foto cover
                            Order = index + 1
                        }).ToList();

                        createdEvent.Images = images;
                    }


                var qrCodeBytes = await qrCodeService.GenerateQrCodeAsync(createdEvent.CheckInToken.Value);
                var qrFileName = $"event-{createdEvent.Id}-qr.png";
                qrUrl = await storageService.UploadFileAsync(qrCodeBytes, qrFileName);
                createdEvent.QrCodeUrl = qrUrl;

                // 5. GOOGLE SHEETS İŞLEMİ
                try
                {
                    sheetsId = await googleSheetCreationService.CreateSheetAsync(createdEvent.Id.ToString(), createdEvent.Title);
                    createdEvent.SheetsId = sheetsId;
                }
                catch (Exception ex)
                {
                }

                // 6. DB GÜNCELLEMESİ (QR URL ve Sheets ID'yi kaydetme)
                await eventRepository.UpdateAsync(createdEvent);

                var message = await localization.Get(ValidationKeys.EventCreatedNotification, createdEvent.Title, clubName);

                // 7. BİLDİRİM GÖNDERME
                await notificationProducer.ClubIsCreateEventAsync(clubId.Value, message);

                transactionService.Commit();
            }
            catch (Exception ex)
            {
                transactionService.Rollback();
                return ServiceResponse<CreateEventResponseDTO>.Fail(await localization.Get(ValidationKeys.EventCreationFailed));
            }

            // 8. BAŞARI YANITI
            return ServiceResponse<CreateEventResponseDTO>.Success(await localization.Get(ValidationKeys.EventCreatedSuccess));
        }
    }
}