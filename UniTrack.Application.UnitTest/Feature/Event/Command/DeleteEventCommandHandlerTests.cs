using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Like.Command; // 💡 Namespace kontrol et, sendeki klasörleme yoluna göre güncelle
using UniTrack.Domain.Entities;
using Xunit;

public class DeleteLikedCommentCommandHandlerTests
{
    private readonly Mock<ICurrentUserServices> _currentUser;
    private readonly Mock<ILikeRepository> _likeRepo;
    private readonly Mock<ICommentRepository> _commentRepo;
    private readonly Mock<ILocalizationService> _localization;
    private readonly DeleteLikedCommentCommandHandler _handler;

    public DeleteLikedCommentCommandHandlerTests()
    {
        _currentUser = new Mock<ICurrentUserServices>();
        _likeRepo = new Mock<ILikeRepository>();
        _commentRepo = new Mock<ICommentRepository>();
        _localization = new Mock<ILocalizationService>();

        _handler = new DeleteLikedCommentCommandHandler(
            _currentUser.Object,
            _likeRepo.Object,
            _commentRepo.Object,
            _localization.Object
        );
    }

// TO DO: Yapılacak
}