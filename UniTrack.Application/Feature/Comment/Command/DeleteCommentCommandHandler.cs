using Castle.Components.DictionaryAdapter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Comment.Command
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICommentRepository commentRepository;
        private readonly ILocalizationService localizationService;

        public DeleteCommentCommandHandler(ICurrentUserServices currentUserServices,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.commentRepository = commentRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            // 1. Veritabanından silinmek istenen yorumu çekiyoruz
            var comment = await commentRepository.GetCommentIdAsync(request.CommentId);

            if (comment == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.CommentNotFound)
                };
            }

            // 2. Mevcut kullanıcının Rolünü ve ID'sini alıyoruz
            var currentRole = currentUserServices.Role();
            var currentUserId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();

            // 3. Yetki Kontrolü (Admin her şeyi silebilir, User ve Club sadece kendi yorumunu silebilir)
            bool isAuthorized = false;

            if (currentRole == Role.Admin)
            {
                isAuthorized = true;
            }
            else if (currentRole == Role.User && comment.UserId == currentUserId)
            {
                isAuthorized = true;
            }
            else if (currentRole == Role.Club && comment.ClubId == clubId)
            {
                isAuthorized = true;
            }

            // Yetkisiz erişim durumu
            if (!isAuthorized)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            // 4. Silme İşlemi
            await commentRepository.DeleteCommentWithLikesAsync(comment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.CommentDeleted),
                Data = null
            };
        }
    }
}