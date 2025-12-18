using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Feature.University.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

namespace UniTrack.Application.Tests.Feature.University.Command
{
    public class DeleteUniversityCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserService;
        private readonly Mock<IUniversityRepository> _universityRepository;
        private readonly Mock<ILocalizationService> _localizationService;
        private readonly DeleteUniversityCommandHandler _handler;

        public DeleteUniversityCommandHandlerTests()
        {
            _currentUserService = new Mock<ICurrentUserServices>();
            _universityRepository = new Mock<IUniversityRepository>();
            _localizationService = new Mock<ILocalizationService>();

            _handler = new DeleteUniversityCommandHandler(
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

            var command = new DeleteUniversityCommand();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_UserNotAdmin_ShouldReturnNotAuthorized()
        {
            _currentUserService.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserService.Setup(x => x.Role()).Returns(Role.User);
            _localizationService.Setup(x => x.Get(It.IsAny<string>()))
                                .ReturnsAsync("NotAuthorized");

            var command = new DeleteUniversityCommand();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_UniversityNotFound_ShouldReturnError()
        {
            _currentUserService.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserService.Setup(x => x.Role()).Returns(Role.Admin);
            _universityRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                                 .ReturnsAsync((Domain.Entities.University)null);
            _localizationService.Setup(x => x.Get(It.IsAny<string>()))
                                .ReturnsAsync("UniversityNotFound");

            var command = new DeleteUniversityCommand
            {
                Id = Guid.NewGuid()
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ValidAdmin_ShouldDeleteUniversitySuccessfully()
        {
            _currentUserService.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserService.Setup(x => x.Role()).Returns(Role.Admin);
            _universityRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                                 .ReturnsAsync(new Domain.Entities.University());
            _localizationService.Setup(x => x.Get(It.IsAny<string>()))
                                .ReturnsAsync("UniversityDeleted");

            var command = new DeleteUniversityCommand
            {
                Id = Guid.NewGuid()
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _universityRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.University>()), Times.Once);
        }
    }
}
