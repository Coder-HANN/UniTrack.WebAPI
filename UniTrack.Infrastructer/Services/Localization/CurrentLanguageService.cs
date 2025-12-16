using Microsoft.AspNetCore.Http;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;

namespace UniTrack.Infrastructure.Localization
{
    public class CurrentLanguageService : ICurrentLanguageService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserServices currentUserServices;

        public CurrentLanguageService(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            ICurrentUserServices currentUserServices)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userRepository = userRepository;
            this.currentUserServices = currentUserServices;
        }

        public async Task<string> GetCultureAsync()
        {
            var userId = currentUserServices.CurrentUser();
            if (userId != null)
            {
                var user = await userRepository.GetByIdAsync(userId.Value);
                if (!string.IsNullOrWhiteSpace(user?.UserDetail.Language))
                    return user.UserDetail.Language;
            }

            var headerLang = httpContextAccessor.HttpContext?
                .Request.Headers["Accept-Language"].ToString();

            if (!string.IsNullOrEmpty(headerLang))
                return headerLang
                    .Split(',')
                    .First()
                    .Split(';')
                    .First();

            return "tr-TR";
        }
    }
}
