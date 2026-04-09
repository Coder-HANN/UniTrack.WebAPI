using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using UniTrack.Application.Feature.Contect.Command;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniTrack.Application.Tests.Feature.Contect.Command
{
    public class SendContactMessageCommandHandlerTests
    {
        private readonly Mock<IMailService> _mailServiceMock = new();
        private readonly Mock<ILocalizationService> _localizationServiceMock = new();
        private readonly Mock<ICurrentUserServices> _currentUserServicesMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly IConfiguration _configuration;

        public SendContactMessageCommandHandlerTests()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "MailSettings:FromEmail", "support@unitrack.com" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        private SendContactMessageCommandHandler CreateHandler(IConfiguration config = null)
            => new SendContactMessageCommandHandler(
                _mailServiceMock.Object,
                config ?? _configuration,
                _localizationServiceMock.Object,
                _currentUserServicesMock.Object,
                _userRepositoryMock.Object);

        [Fact]
        public async Task Handle_ValidRequest_ShouldSendMailAndReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@user.com",
                UserDetail = new UserDetail { Name = "Ali", Surname = "Veli" }
            };

            _currentUserServicesMock.Setup(x => x.CurrentUser()).Returns(userId);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _localizationServiceMock.Setup(x => x.Get(ValidationKeys.ContactMessageSent))
                .ReturnsAsync("Mesaj başarıyla gönderildi");

            var command = new SendContactMessageCommand { Subject = "İletişim", Message = "Merhaba" };
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mailServiceMock.Verify(x => x.SendMailAsync(
                "support@unitrack.com",
                It.Is<string>(s => s.Contains("[Contact]")),
                It.Is<string>(b => b.Contains("Ali Veli") && b.Contains("test@user.com")),
                true,
                null), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _currentUserServicesMock.Setup(x => x.CurrentUser()).Returns(userId);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null);
            _localizationServiceMock.Setup(x => x.Get(ValidationKeys.UserNotFound))
                .ReturnsAsync("Kullanıcı bulunamadı");

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new SendContactMessageCommand(), default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Kullanıcı bulunamadı");
            _mailServiceMock.Verify(x => x.SendMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), null), Times.Never);
        }

        [Fact]
        public async Task Handle_SupportEmailNotConfigured_ShouldReturnError()
        {
            // Arrange
            var emptyConfig = new ConfigurationBuilder().Build();
            _localizationServiceMock.Setup(x => x.Get(ValidationKeys.SupportEmailNotConfigured))
                .ReturnsAsync("Destek maili tanımlı değil");

            var handler = CreateHandler(emptyConfig);

            // Act
            var result = await handler.Handle(new SendContactMessageCommand(), default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Destek maili tanımlı değil");
        }
    }
}