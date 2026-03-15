using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Logger;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Abstraction.Services.MemoryCach;
using UniTrack.Application.Abstraction.Services.Notification;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Abstraction.Services.UserHub;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Feature.Auth.Command;
using UniTrack.Domain.Entities;
using UniTrack.Infrastructure.Localization;
using UniTrack.Infrastructure.Services;
using UniTrack.Infrastructure.Services.Background;
using UniTrack.Infrastructure.Services.Sheets;
using UniTrack.Persistence.Context;
using UniTrack.Persistence.Repositories;
using UniTrack.WebAPI.Extensions;
using UniTrack.WebAPI.Middleware;

// 1. Claim isimlerinin bozulmaması için bu satır EN TEPEDE kalmalı
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins(
                "https://localhost:4200",
                "http://localhost:4200"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddInfrastructureServices(configuration);

var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UniTrackDbContext>(options => options.UseSqlServer(connectionString));

var applicationAssembly = typeof(LoginCommand).Assembly;
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly, applicationAssembly));
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(applicationAssembly));


builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Common/Localization/Resources";
});

builder.Services.AddScoped<ICurrentLanguageService, CurrentLanguageService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<UniTrackDbContext>());
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserServices, CurrentUserServices>();
builder.Services.AddScoped<IEventQuestionRepository, EventQuestionEntityRepository>();

// Redis Bağlantısı (Singleton olması performans için iyidir)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379"));

// Producer Servisi
builder.Services.AddScoped<INotificationService, NotificationService>();

// Repositories
builder.Services.AddScoped(typeof(IBaseEntityRepository<>), typeof(BaseEntityRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDetailRepository, UserDetailRepository>();
builder.Services.AddScoped<IClubRepository, ClubEntityRepository>();
builder.Services.AddScoped<IEventRepository, EventEntityRepository>();
builder.Services.AddScoped<IEventUserRepository, EventUserRepository>();
builder.Services.AddScoped<IUserClubRepository, UserClubRepository>();
builder.Services.AddScoped<ICommentRepository, CommentEntityRepository>();
builder.Services.AddScoped<IBanRepository, BanRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IClubTeamRepository, ClubTeamRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<IStorageService, FakeStorageService>();
builder.Services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
builder.Services.AddScoped<IClubNotificationRepository, ClubNotificationRepository>();
builder.Services.AddScoped<ITargetNotificationCityRepository, TargetNotificationCityRepository>();
builder.Services.AddScoped<ITargetNotificationDepartmentRepository, TargetNotificationDepartmentRepository>();
builder.Services.AddScoped<ITargetNotificationRepository, TargetNotificationRepository>();
builder.Services.AddScoped<ITargetNotificationUniversityRepository,  TargetNotificationUniversityRepository>();
builder.Services.AddScoped<ITargetNotificationClubRepository, TargetNotificationClubRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IAdminNotificationRepository, AdminNotificationRepository>();
builder.Services.AddScoped<IEventImageRepository, EventImageRepository>();
builder.Services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
builder.Services.AddSingleton<IBackgroundMailQueue, BackgroundMailQueue>();
builder.Services.AddScoped<IMailNotificationService, MailNotificationService>();
builder.Services.AddHostedService<EventTimeUpdateBackgroundService>();

builder.Services.AddHostedService<MailWorker>();

builder.Services.AddScoped<IEventQuestionAnswerRepository, EventQuestionAnswerEntityRepository>();


builder.Services.AddSignalR();

builder.Services.AddScoped<IUserRegisterCountService, RegistrationNotifierService>(); 
// Services
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();

// Veya AddTransient kullanabilirsiniz.

// 2. Identity Servisleri (JWT Ayarlarını Yükler - 1. Adımdaki Dosya)
builder.Services.AddIdentityServices(configuration);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHasher<Club>, PasswordHasher<Club>>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheManager, MemoryCacheManager>();

builder.Services.AddSingleton<LoggerServiceBase, MsSqlLogger>();

builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Seed Data
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UniTrackDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    var email = config["AdminUser:Email"];
    var password = config["AdminUser:Password"];

    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
    {
        if (!await dbContext.Users.AnyAsync(u => u.Email == email && u.Role == UniTrack.Domain.Enums.Role.Admin))
        {
            dbContext.Users.Add(new User
            {
                Email = email,
                Role = UniTrack.Domain.Enums.Role.Admin,
                Password = passwordHasher.HashPassword(null!, password),
                CreatedDate = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

var supportedCultures = new[] { "tr-TR", "en-US" };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("tr-TR"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList()
};

app.UseRequestLocalization(localizationOptions);

// 🔥 ROUTING MUTLAKA BURADA OLMALI
app.UseRouting();

app.UseCors("DevCors");
// 🔐 AUTH
app.UseAuthentication();
app.UseAuthorization();

// 🧱 CUSTOM MIDDLEWARE (Auth sonrası doğru)
app.UseMiddleware<AdvancedLoggingMiddleware>();
app.UseMiddleware<BanMiddleware>();
app.UseMiddleware<UserMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();

// 🎯 ENDPOINT TANIMLARI (NET 6+)
app.MapControllers();
app.MapHub<UserCountHub>("/userCountHub");
app.MapHub<NotificationHub>("/notificationhub");

app.Run();