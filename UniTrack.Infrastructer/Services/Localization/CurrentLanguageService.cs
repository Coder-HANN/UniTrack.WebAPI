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

        public string GetCulture()
        {
            // 1️⃣ User profile
            var userId = currentUserServices.CurrentUser();
            if (userId != null)
            {
                var user = userRepository.GetByIdAsync(userId.Value).Result;
                if (!string.IsNullOrWhiteSpace(user?.UserDetail.Language))
                    return user.UserDetail.Language;
            }

            // 2️⃣ Accept-Language
            var headerLang = httpContextAccessor.HttpContext?
                .Request.Headers["Accept-Language"].ToString();

            if (!string.IsNullOrEmpty(headerLang))
                return headerLang.Split(',').First();

            // 3️⃣ Default
            return "tr-TR";
        }
    }
}
