using Microsoft.OpenApi.Models;

namespace UniTrack.WebAPI.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Mevcut API Tanımlamaları
                c.SwaggerDoc("Admin", new OpenApiInfo { Title = "Admin API", Version = "v1" });
                c.SwaggerDoc("Public", new OpenApiInfo { Title = "Public API", Version = "v1" });
                c.SwaggerDoc("Club", new OpenApiInfo { Title = "Club API", Version = "v1" });
                c.SwaggerDoc("SuperAdmin", new OpenApiInfo { Title = "SuperAdmin API", Version = "v1" });

                // 1. Bearer Şemasını Tanımla (Security Definition)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
                });

                // 2. Bu Şemanın Kullanılacağını Belirt (Security Requirement)
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Admin/swagger.json", "Admin API");
                c.SwaggerEndpoint("/swagger/Public/swagger.json", "Public API");
                c.SwaggerEndpoint("/swagger/Club/swagger.json", "Club API");
            });

            return app;
        }
    }
}