using AutoMapper;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Feature.Event.Command; // Handler'ın olduğu namespace
using UniTrack.Domain.Enums;    // Role enum'ı burada varsayalım
using Xunit;

namespace UniTrack.Application.UnitTests.Feature.Event.Command
{
    public class CreateEventCommandHandlerTests
    {
        // 1. Kullanacağımız tüm bağımlılıkları Mock olarak tanımlıyoruz
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<INotificationService> _notificationProducerMock;

        // Test edeceğimiz ana sınıf (System Under Test)
        private readonly CreateEventCommandHandler _handler;

        public CreateEventCommandHandlerTests()
        {
            // 2. Mock nesnelerini başlatıyoruz
            _currentUserServiceMock = new Mock<ICurrentUserServices>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _mapperMock = new Mock<IMapper>();
            _notificationProducerMock = new Mock<INotificationService>();

            // 3. Handler'ı mock bağımlılıklarla ayağa kaldırıyoruz
            _handler = new CreateEventCommandHandler(
                _currentUserServiceMock.Object,
                _eventRepositoryMock.Object,
                _mapperMock.Object,
                _notificationProducerMock.Object
            );
        }

        // SENARYO 1: Kullanıcı ve Kulüp ID null ise "Unauthorized" dönmeli
        [Fact]
        public async Task Handle_Should_Return_Unauthorized_When_User_And_Club_Are_Null()
        {
            // --- ARRANGE (Hazırlık) ---
            // CurrentUserServices mock'unu ayarla: Hem User hem Club null dönsün
          //  _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns((Guid)null);
            //_currentUserServiceMock.Setup(x => x.CurrentClub()).Returns((Guid)null);

            var command = new CreateEventCommand(); // İçi boş olabilir, buraya gelmeden patlayacak zaten

            // --- ACT (Eylem) ---
            var result = await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT (Doğrulama) ---
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Unauthorized");

            // ÖNEMLİ: Yetkisiz olduğu için Repository'nin Add metodu ASLA çağrılmamalı
            _eventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UniTrack.Domain.Entities.Event>()), Times.Never);
        }

        // SENARYO 2: Rol "User" ise "Yetkisiz kullanıcı" dönmeli
        [Fact]
        public async Task Handle_Should_Return_Error_When_Role_Is_User()
        {
            // --- ARRANGE ---
            // Kullanıcı ID var ama Rolü sadece 'User'
           // _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns("user-123");
            _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

            var command = new CreateEventCommand();

            // --- ACT ---
            var result = await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Yetkisiz kullanıcı");

            // Repository yine çağrılmamalı
            _eventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UniTrack.Domain.Entities.Event>()), Times.Never);
        }

        // SENARYO 3: Her şey doğruysa (Happy Path) Başarılı dönmeli ve Kayıt yapmalı
        [Fact]
        public async Task Handle_Should_Create_Event_When_User_Is_Authorized()
        {
            // --- ARRANGE ---
            // 1. Yetki ayarları: Admin olsun
           // _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns("admin-123");
            _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.Admin); // veya ClubAdmin

            // 2. Mapper ayarı: Command gelince bize boş bir Event entity dönsün
            var command = new CreateEventCommand { Title = "Test Event" };
            var eventEntity = new UniTrack.Domain.Entities.Event { Title = "Test Event" };

            _mapperMock.Setup(m => m.Map<UniTrack.Domain.Entities.Event>(command))
                       .Returns(eventEntity);

            // 3. Repository ayarı: AddAsync çağrılınca başarılı dönsün
            _eventRepositoryMock.Setup(r => r.AddAsync(It.IsAny<UniTrack.Domain.Entities.Event>()))
                                .ReturnsAsync(eventEntity);

            // --- ACT ---
            var result = await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("Event created successfully");

            // KRİTİK KONTROL: Repository'nin AddAsync metodu, maplenmiş entity ile TAM BİR KERE çağrıldı mı?
            _eventRepositoryMock.Verify(x => x.AddAsync(eventEntity), Times.Once);
        }
    }
}