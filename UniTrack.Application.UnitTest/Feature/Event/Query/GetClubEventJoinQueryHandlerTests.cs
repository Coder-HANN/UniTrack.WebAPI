using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;
using FluentAssertions; // Daha akıcı assertionlar için tavsiye ederim

public class GetClubEventJoinQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock;
    private readonly Mock<ILocalizationService> _localizationMock;
    private readonly GetClubEventJoinQueryHandler _handler;

    public GetClubEventJoinQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventUserRepositoryMock = new Mock<IEventUserRepository>();
        _localizationMock = new Mock<ILocalizationService>();

        _handler = new GetClubEventJoinQueryHandler(
            _currentUserMock.Object,
            _eventUserRepositoryMock.Object,
            _localizationMock.Object // Localization servisi eklendi
        );
    }

    [Fact]
    public async Task Handle_UserAndClubNull_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Yetkisiz Erişim");

        // Act
        var result = await _handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Yetkisiz Erişim", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_RoleUser_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.NotAuthorized))
            .ReturnsAsync("Yetkisiz Erişim");

        // Act
        var result = await _handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Yetkisiz Erişim", result.Message);
    }

    [Fact]
    public async Task Handle_NoEventJoins_ShouldReturnFail()
    {
        // Arrange
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

        _eventUserRepositoryMock
            .Setup(x => x.GetClubEventJoinsByClubIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((List<EventUser>)null);

        _localizationMock
            .Setup(x => x.Get(ValidationKeys.EventNotFound))
            .ReturnsAsync("Kayıt bulunamadı");

        // Act
        var result = await _handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Kayıt bulunamadı", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnUserList()
    {
        // Arrange
        var clubId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(clubId);
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

        var eventUsers = new List<EventUser>
        {
            new EventUser
            {
                User = new User
                {
                    UserDetail = new UserDetail
                    {
                        Name = "Ali",
                        Surname = "Veli",
                        UniverstiyId = Guid.NewGuid(),
                        DepartmentId = 1
                    }
                }
            }
        };

        _eventUserRepositoryMock
            .Setup(x => x.GetClubEventJoinsByClubIdAsync(clubId))
            .ReturnsAsync(eventUsers);

        // Act
        var result = await _handler.Handle(new GetClubEventJoinQuery(clubId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Null(result.Message); // Başarı durumunda genelde mesaj null veya "Success" döner
    }
}