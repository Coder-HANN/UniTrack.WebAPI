using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Feature.University.Command;
using UniTrack.Domain.Enums;
using Xunit;

namespace UniTrack.Application.Tests.Feature.University.Command
{
    public class CreateUniversityCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserService;
        private readonly Mock<IUniversityRepository> _universityRepository;
        private readonly Mock<ILocalizationService> _localizationService;
        private readonly CreateUniversityCommandHandler _handler;

        public CreateUniversityCommandHandlerTests()
        {
            _currentUserService = new Mock<ICurrentUserServices>();
            _universityRepository = new Mock<IUniversityRepository>();
            _localizationService = new Mock<ILocalizationService>();

            _handler = new CreateUniversityCommandHandler(
                _currentUserService.Object,
                _universityRepository.Object,
                _localizationService.Object);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_ShouldReturnNotAuthorized()
        {
            _currentUserService.Setup(x => x.CurrentUser()).Returns((Guid?)null);
            _localizationService.Setup(x => x.Get(It.IsAny<string>()))
                                .ReturnsAsync("NotAuthorized");

            var command = new CreateUniversityCommand();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("NotAuthorized");
        }

        [Fact]
        public async Task Handle_UserRoleIsUser_ShouldReturnNotAuthorized()
        {
            _currentUserService.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserService.Setup(x => x.Role()).Returns(Role.User);
            _localizationService.Setup(x => x.Get(It.IsAny<string>()))
                                .ReturnsAsync("NotAuthorized");

            var command = new CreateUniversityCommand();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_AdminUser_ShouldCreateUniversitySuccessfully()
        {
            _currentUserService.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserService.Setup(x => x.Role()).Returns(Role.Admin);
            _localizationService.Setup(x => x.Get(It.IsAny<string>()))
                                .ReturnsAsync("UniversityCreated");

            var command = new CreateUniversityCommand
            {
                Name = "Test University",
                CityId = 34
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _universityRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.University>()), Times.Once);
        }
    }
}
