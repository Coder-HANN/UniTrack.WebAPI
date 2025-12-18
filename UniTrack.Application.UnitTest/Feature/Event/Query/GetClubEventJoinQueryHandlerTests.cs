using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class GetClubEventJoinQueryHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUserMock;
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock;
    private readonly GetClubEventJoinQueryHandler _handler;

    public GetClubEventJoinQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUserServices>();
        _eventUserRepositoryMock = new Mock<IEventUserRepository>();

        _handler = new GetClubEventJoinQueryHandler(
            _currentUserMock.Object,
            _eventUserRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_UserAndClubNull_ShouldReturnUnauthorized()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns((Guid?)null);
        _currentUserMock.Setup(x => x.CurrentClub()).Returns((Guid?)null);

        var result = await _handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_RoleUser_ShouldReturnUnauthorized()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.User);

        var result = await _handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_NoEventJoins_ShouldReturnFail()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Role()).Returns(Role.Club);

        _eventUserRepositoryMock
            .Setup(x => x.GetClubEventJoinsByClubIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((List<EventUser>)null);

        var result = await _handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnUserList()
    {
        _currentUserMock.Setup(x => x.CurrentUser()).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.CurrentClub()).Returns(Guid.NewGuid());
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
            .Setup(x => x.GetClubEventJoinsByClubIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(eventUsers);

        var result = await _handler.Handle(new GetClubEventJoinQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }
}
