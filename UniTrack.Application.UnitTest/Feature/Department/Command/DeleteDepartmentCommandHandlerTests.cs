using Xunit;
using Moq;
using FluentAssertions;
using UniTrack.Application.Feature.Department.Command;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Tests.Feature.Department.Command
{
    public class DeleteDepartmentCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly DeleteDepartmentCommandHandler _handler;

        public DeleteDepartmentCommandHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserServices>();
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new DeleteDepartmentCommandHandler(
                _currentUserServiceMock.Object,
                _departmentRepositoryMock.Object,
                _localizationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns((Guid?)null);

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Yetkisiz");

            var command = new DeleteDepartmentCommand
            {
                Id = 1,
                Name = "Computer Engineering"
            };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Yetkisiz");

            _departmentRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<UniTrack.Domain.Entities.Department>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UserIsNotAdmin_ShouldReturnNotAuthorized()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.User);

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Yetkisiz");

            var command = new DeleteDepartmentCommand
            {
                Id = 1,
                Name = "Computer Engineering"
            };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Yetkisiz");

            _departmentRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<UniTrack.Domain.Entities.Department>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_DepartmentNotFound_ShouldReturnError()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.Admin);

            _departmentRepositoryMock.Setup(x =>
                x.GetByIdAsync(1, "Computer Engineering"))
                .ReturnsAsync((UniTrack.Domain.Entities.Department)null);

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.DepartmentNotFound))
                .ReturnsAsync("Bölüm bulunamadı");

            var command = new DeleteDepartmentCommand
            {
                Id = 1,
                Name = "Computer Engineering"
            };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Bölüm bulunamadı");

            _departmentRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<UniTrack.Domain.Entities.Department>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ValidAdminRequest_ShouldDeleteDepartment()
        {
            // Arrange
            var department = new UniTrack.Domain.Entities.Department
            {
                Id = 1,
                Name = "Computer Engineering"
            };

            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.Admin);

            _departmentRepositoryMock.Setup(x =>
                x.GetByIdAsync(1, "Computer Engineering"))
                .ReturnsAsync(department);

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.DepartmentDeletedSuccessfully))
                .ReturnsAsync("Bölüm silindi");

            var command = new DeleteDepartmentCommand
            {
                Id = 1,
                Name = "Computer Engineering"
            };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Bölüm silindi");

            _departmentRepositoryMock.Verify(
                x => x.DeleteAsync(department),
                Times.Once);
        }
    }
}
