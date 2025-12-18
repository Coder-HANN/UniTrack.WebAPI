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
    public class CreateDepartmentCommandHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly CreateDepartmentCommandHandler _handler;

        public CreateDepartmentCommandHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserServices>();
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new CreateDepartmentCommandHandler(
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

            var command = new CreateDepartmentCommand { Name = "Computer Engineering" };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Yetkisiz");

            _departmentRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Domain.Entities.Department>()),
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

            var command = new CreateDepartmentCommand { Name = "Computer Engineering" };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Yetkisiz");

            _departmentRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Domain.Entities.Department>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_DepartmentAlreadyExists_ShouldReturnError()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.Admin);

            _departmentRepositoryMock.Setup(x =>
                x.GetDepartmentByNameAsync("Computer Engineering"))
                .ReturnsAsync(new Domain.Entities.Department { Name = "Computer Engineering" });

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.DepartmentAlreadyExists))
                .ReturnsAsync("Bölüm zaten mevcut");

            var command = new CreateDepartmentCommand { Name = "Computer Engineering" };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Bölüm zaten mevcut");

            _departmentRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Domain.Entities.Department>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ValidAdminRequest_ShouldCreateDepartment()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.CurrentUser())
                .Returns(Guid.NewGuid());

            _currentUserServiceMock.Setup(x => x.Role())
                .Returns(Role.Admin);

            _departmentRepositoryMock.Setup(x =>
                x.GetDepartmentByNameAsync("Computer Engineering"))
                .ReturnsAsync((Domain.Entities.Department)null);

            _localizationServiceMock.Setup(x =>
                x.Get(ValidationKeys.DepartmentCreatedSuccessfully))
                .ReturnsAsync("Bölüm başarıyla oluşturuldu");

            var command = new CreateDepartmentCommand
            {
                Name = "Computer Engineering"
            };

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Computer Engineering");
            result.Message.Should().Be("Bölüm başarıyla oluşturuldu");

            _departmentRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Domain.Entities.Department>(d => d.Name == "Computer Engineering")),
                Times.Once);
        }
    }
}
