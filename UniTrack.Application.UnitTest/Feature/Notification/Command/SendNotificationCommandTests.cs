using FluentValidation.TestHelper;
using UniTrack.Application.Feature.Notification.Command;
using UniTrack.Application.Feature.Notification.Validator;
using UniTrack.Domain.Enums;
using Xunit;

namespace UniTrack.Application.UnitTest.Feature.Notification.Command
{
   
    public class SendNotificationCommandTests
    {
        private readonly SendNotificationCommandValidator _validator;

        public SendNotificationCommandTests()
        {
            _validator = new SendNotificationCommandValidator();
        }

        [Fact]
        public void Should_Fail_When_Channel_Is_Empty()
        {
            var command = new SendNotificationCommand
            {
                Title = "Test",
                Message = "Test message",
                Channels = new List<NotificationChannel>()
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Channels);
        }

        [Fact]
        public void Should_Fail_When_Message_Is_Empty()
        {
            var command = new SendNotificationCommand
            {
                Title = "Test",
                Message = "",
                Channels = new List<NotificationChannel> { NotificationChannel.Email }
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Message);
        }

        [Fact]
        public void Should_Pass_With_Valid_Command()
        {
            var command = new SendNotificationCommand
            {
                Title = "Test",
                Message = "Hello users",
                CityIds = new List<int> { 34, 6 },
                UniversityIds = new List<Guid> { Guid.NewGuid() },
                Channels = new List<NotificationChannel>
            {
                NotificationChannel.Email
            }
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

}
