using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3; 
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniTrack.Application.Abstraction.Services.QrCode;
using UniTrack.Application.Abstraction.Services.Sheets;
using UniTrack.Infrastructure.Services.QrCode;

namespace UniTrack.Infrastructure.Services.Sheets
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var sheetsConfig = configuration.GetSection("GoogleSheets").Get<GoogleSheetsConfig>();

            // RootFolderId kontrolü eklendi
            if (sheetsConfig == null || string.IsNullOrEmpty(sheetsConfig.ServiceAccountJsonPath) || string.IsNullOrEmpty(sheetsConfig.RootFolderId))
            {
                return services;
            }

            // 1. GoogleCredential'ı Singleton olarak kaydet (İki API için de yetki)
            // YENİ
            services.AddSingleton<UserCredential>(provider =>
            {
                using var stream = new FileStream(
                    sheetsConfig.ServiceAccountJsonPath, FileMode.Open, FileAccess.Read);

                var scopes = new[] { GoogleSheetsConfig.SheetsScope, GoogleSheetsConfig.DriveScope };

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(sheetsConfig.TokenPath, fullPath: true)
                ).GetAwaiter().GetResult();
            });
            // 2. SheetsService'i Singleton olarak kaydet (Mevcut)
            services.AddSingleton<SheetsService>(provider =>
            {
                var credential = provider.GetRequiredService<UserCredential>();
                return new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "UniTrack CampusConnect App",
                });
            });

            // 3. DriveService'i Singleton olarak kaydet (YENİ EKLEME)
            services.AddSingleton<DriveService>(provider =>
            {
                var credential = provider.GetRequiredService<GoogleCredential>();
                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "UniTrack CampusConnect App",
                });
            });

            services.AddScoped<IQrCodeService, QrCodeService>();

            return services;
        }
    }
}