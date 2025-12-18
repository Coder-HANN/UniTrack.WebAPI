using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using UniTrack.Application.Feature.Contect.Command;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Tests.Feature.Contect.Command
{
    public class SendContactMessageCommandHandlerTests
    {
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;
        private readonly IConfiguration _configuration;

        private readonly SendContactMessageCommandHandler _handler;

        public SendContactMessageCommandHandlerTests()
        {
            _mailServiceMock = new Mock<IMailService>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            // In-Memory Configuration (Gerçek config yok)
            var inMemorySettings = new Dictionary<string, string>
            {
                { "MailSettings:FromEmail", "support@unitrack.com" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _handler = new SendContactMessageCommandHandler(
                _mailServiceMock.Object,
                _configuration,
                _localizationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_SupportEmailNotConfigured_ShouldReturnError()
        {
            // Arrange
            var emptyConfig = new ConfigurationBuilder().Build();

            var handler = new SendContactMessageCommandHandler(
                _mailServiceMock.Object,
                emptyConfig,
                _localizationServiceMock.Object);

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.SupportEmailNotConfigured))
                .ReturnsAsync("Destek maili tanımlı değil");

            var command = new SendContactMessageCommand
            {
                Name = "Ali Veli",
                Email = "ali@test.com",
                Subject = "Test",
                Message = "Deneme"
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Destek maili tanımlı değil");

            _mailServiceMock.Verify(
                 x => x.SendMailAsync(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     It.IsAny<List<(Stream Stream, string FileName)>>()),
                 Times.Never
            );

        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldSendMailAndReturnSuccess()
        {
            // Arrange
            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.ContactMessageSent))
                .ReturnsAsync("Mesaj başarıyla gönderildi");

            var command = new SendContactMessageCommand
            {
                Name = "Ali Veli",
                Email = "ali@test.com",
                Subject = "İletişim",
                Message = "Merhaba, destek almak istiyorum."
            };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Mesaj başarıyla gönderildi");

             _mailServiceMock.Verify(x =>
             x.SendMailAsync(
                 "support@unitrack.com",
                 It.Is<string>(s => s.Contains("[Contact]")),
                 It.Is<string>(b => b.Contains("Ali Veli")),
                 true,
                 null),
             Times.Once);
        }
    }
}
