using AutoMapper;
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Abstraction.Services.QrCode;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Application.Abstraction.Services.Storage;
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

        public CreateEventCommandHandler(
            ICurrentUserServices currentUserServices,
            IEventRepository eventRepository,
            IMapper mapper,
            INotificationService notificationProducer,
            IGoogleSheetCreationService googleSheetCreationService,
            IQrCodeService qrCodeService,
            IStorageService storageService,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.eventRepository = eventRepository;
            this.mapper = mapper;
            this.notificationProducer = notificationProducer;
            this.googleSheetCreationService = googleSheetCreationService;
            this.qrCodeService = qrCodeService;
            this.storageService = storageService;
            this.localization = localizationService;
        }

        public async Task<ServiceResponse<CreateEventResponseDTO>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();
            var role = currentUserServices.Role();

            if (userId == null && clubId == null || role == null || role == Role.User)
            {
                return ServiceResponse<CreateEventResponseDTO>.Fail(
                     localization.Get(ValidationKeys.NotAuthorized));
            }

            var newEvent = mapper.Map<Domain.Entities.Event>(request);

            // 2. CHECK-IN TOKEN OLUŞTURMA
            var checkInToken = Guid.NewGuid();
            newEvent.CheckInToken = checkInToken;

            // 3. ETKİNLİĞİ DB'YE KAYDET (ID'yi almak için)
            var createdEvent = await eventRepository.AddAsync(newEvent);

            string? qrUrl = null;
            string? sheetsId = null;

            // 4. QR KODU OLUŞTURMA VE DEPOLAMA
            
                // A. QR Görüntüsünü Byte Olarak Oluştur
                var qrCodeBytes = await qrCodeService.GenerateQrCodeAsync(createdEvent.CheckInToken.Value);

                // B. Benzersiz Dosya Adı Oluştur
                var qrFileName = $"event-{createdEvent.Id}-qr.png";

                // C. Görüntüyü Depolama Servisine Yükle ve URL'yi al
                qrUrl = await storageService.UploadFileAsync(qrCodeBytes, qrFileName);
                createdEvent.QrCodeUrl = qrUrl;
            

            // 5. GOOGLE SHEETS İŞLEMİ
            
                // E-Tablo oluşturuluyor ve ID'si alınıyor.
                sheetsId = await googleSheetCreationService.CreateSheetAsync(createdEvent.Title);
                createdEvent.SheetsId = sheetsId;
            
            // 6. DB GÜNCELLEMESİ (QR URL ve Sheets ID'yi kaydetme)
            // En az biri başarıyla oluşturulup entity'ye atanmışsa (null değilse) DB'yi güncelliyoruz.
                await eventRepository.UpdateAsync(createdEvent);


            // 7. BİLDİRİM GÖNDERME
            await notificationProducer.ClubIsCreateEventAsync(clubId.Value,localization.Get(ValidationKeys.EventCreatedNotification,createdEvent.Title,createdEvent.Club.Name));

            // 8. BAŞARI YANITI
            return ServiceResponse<CreateEventResponseDTO>.Success(localization.Get(ValidationKeys.EventCreatedSuccess));
        }
    }
}