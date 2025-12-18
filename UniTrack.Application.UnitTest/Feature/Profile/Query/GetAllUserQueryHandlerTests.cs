using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.Profile.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

namespace UniTrack.Application.Tests.Feature.Profile.Query
{
    public class GetAllUserQueryHandlerTests
    {
        private readonly Mock<ICurrentUserServices> _currentUserServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILocalizationService> _localizationServiceMock;

        private readonly GetAllUserQueryHandler _handler;

        public GetAllUserQueryHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserServices>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _handler = new GetAllUserQueryHandler(
                _currentUserServiceMock.Object,
                _userRepositoryMock.Object,
                _localizationServiceMock.Object
            );
        }

        // ❌ Kullanıcı yok → Unauthorized
        [Fact]
        public async Task Handle_UserNotLoggedIn_ShouldReturnUnauthorized()
        {
            _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);
            _localizationServiceMock
                .Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Not authorized");

            var result = await _handler.Handle(new GetAllUserQuery(), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Not authorized");
        }

        // ❌ Rol yetkisiz (User / Club)
        [Fact]
        public async Task Handle_InvalidRole_ShouldReturnUnauthorized()
        {
            _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.User);

            _localizationServiceMock
                .Setup(x => x.Get(ValidationKeys.NotAuthorized))
                .ReturnsAsync("Not authorized");

            var result = await _handler.Handle(new GetAllUserQuery(), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Not authorized");
        }

        // ❌ Kullanıcı listesi boş
        [Fact]
        public async Task Handle_NoUsersFound_ShouldReturnFailure()
        {
            _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.Admin);

            _userRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<User>());

            var result = await _handler.Handle(new GetAllUserQuery(), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Kullanıcı bulunamadı.");
        }

        // ✅ Başarılı senaryo
        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnUserList()
        {
            _currentUserServiceMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
            _currentUserServiceMock.Setup(x => x.Role()).Returns(Role.Admin);

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "test1@mail.com",
                    UserDetail = new UserDetail
                    {
                        Name = "Ali",
                        Surname = "Veli",
                        UniverstiyId = Guid.NewGuid(),
                        DepartmentId = 1
                    }
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "test2@mail.com",
                    UserDetail = new UserDetail
                    {
                        Name = "Ayşe",
                        Surname = "Demir",
                        UniverstiyId = Guid.NewGuid(),
                        DepartmentId = 2
                    }
                }
            };

            _userRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(users);

            var result = await _handler.Handle(new GetAllUserQuery(), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Message.Should().Contain("Toplam kullanıcı sayısı");
        }
    }
}
