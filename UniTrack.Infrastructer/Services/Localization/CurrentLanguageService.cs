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
            try
            {
                var userId = currentUserServices.CurrentUser();
                if (userId != null)
                {
                    // DİKKAT: GetByIdAsync içinde UserDetail mutlaka Include edilmiş olmalı!
                    var user = await userRepository.GetByIdAsync(userId.Value);

                    // UserDetail veya Language null ise hata vermemesi için ?. ekledik
                    if (!string.IsNullOrWhiteSpace(user?.UserDetail?.Language))
                        return user.UserDetail.Language;
                }

                // HttpContext kontrolü
                var context = httpContextAccessor.HttpContext;
                if (context == null) return "tr-TR";

                var headerLang = context.Request.Headers["Accept-Language"].ToString();

                if (!string.IsNullOrEmpty(headerLang))
                {
                    return headerLang
                        .Split(',')
                        .First()
                        .Split(';')
                        .First();
                }
            }
            catch
            {
                // Herhangi bir beklenmedik durumda sistemin çökmemesi için varsayılan dil
                return "tr-TR";
            }

            return "tr-TR";
        }
    }
}
