using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Application.Feature.Auth.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResponse<LoginResponseDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly IClubRepository clubRepository;
    private readonly IPasswordHasher<User> userPasswordHasher;
    private readonly IPasswordHasher<Club> clubPasswordHasher;
    private readonly IConfiguration configuration;
    private readonly ILocalizationService localizationService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IClubRepository clubRepository,
        IPasswordHasher<User> userPasswordHasher,
        IPasswordHasher<Club> clubPasswordHasher,
        IConfiguration configuration,
        ILocalizationService localizationService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.userRepository = userRepository;
        this.clubRepository = clubRepository;
        this.userPasswordHasher = userPasswordHasher;
        this.clubPasswordHasher = clubPasswordHasher;
        this.configuration = configuration;
        this.localizationService = localizationService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. USER
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user != null && user.Role == Role.User)
        {
            var result = userPasswordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Success)
            {
                string name = user.UserDetail?.Name ?? "";
                string surname = user.UserDetail?.Surname ?? "";
                var token = GenerateJwtToken(user.Id, Role.User, user.Email, name, surname);

                SetTokenCookie(token, 5);

                user.LastLoginDate = DateTime.UtcNow;
                await userRepository.UpdateAsync(user);

                return new ServiceResponse<LoginResponseDTO>
                {
                    IsSuccess = true,
                    Data = new LoginResponseDTO
                    {

                        Expiration = DateTime.UtcNow.AddHours(5),
                        Role = Role.User.ToString().ToLower(),
                        UserId = user.Id.ToString(),
                        FullName = $"{name} {surname}".Trim(),
                        Email = user.Email,
                        UniversityId = user.UserDetail.UniverstiyId.ToString(),
                        CityId = user.UserDetail.CityId,
                        DepartmentId = user.UserDetail.DepartmentId,
                        Gender = user.UserDetail.Gender != null ? (int)user.UserDetail.Gender : null

                    },
                    Message = await localizationService.Get(ValidationKeys.LoginSuccess)
                };
            }
        }

        // 2. CLUB
        var club = await clubRepository.GetByEmailAsync(request.Email);
        if (club != null && club.Role == Role.Club)
        {
            var result = clubPasswordHasher.VerifyHashedPassword(club, club.Password, request.Password);
            if (result == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(club.Id, Role.Club, club.ContectEmail, name: club.Name);

                SetTokenCookie(token, 5);

                club.LastLoginDate = DateTime.UtcNow;
                await clubRepository.UpdateAsync(club);

                return new ServiceResponse<LoginResponseDTO>
                {
                    IsSuccess = true,
                    Data = new LoginResponseDTO
                    {
                        Expiration = DateTime.UtcNow.AddDays(30),
                        Role = Role.Club.ToString().ToLower(),
                        ClubId = club.Id.ToString(),
                        FullName = club.Name,
                        Email = club.ContectEmail
                    },
                    Message = await localizationService.Get(ValidationKeys.LoginSuccess)
                };
            }
        }

        // 3. ADMIN
        if (user != null && user.Role == Role.Admin)
        {
            var result = userPasswordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Success)
            {
                string name = user.UserDetail?.Name ?? "Admin";
                var token = GenerateJwtToken(user.Id, Role.Admin, user.Email, name);

                SetTokenCookie(token, 1);

                return new ServiceResponse<LoginResponseDTO>
                {
                    IsSuccess = true,
                    Data = new LoginResponseDTO
                    {
                        Expiration = DateTime.UtcNow.AddHours(1),
                        Role = Role.Admin.ToString().ToLower(),
                        UserId = user.Id.ToString(),
                        FullName = name,
                        Email = user.Email
                    },
                    Message = await localizationService.Get(ValidationKeys.LoginSuccess)
                };
            }
        }

        return new ServiceResponse<LoginResponseDTO>
        {
            IsSuccess = false,
            Message = await localizationService.Get(ValidationKeys.InvalidEmailOrPassword)
        };
    }

    private void SetTokenCookie(string token, int hours)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(hours)
        });
    }

    private string GenerateJwtToken(Guid id, Role role, string email, string name = "", string surname = "")
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        if (role == Role.User || role == Role.Admin)
        {
            claims.Add(new Claim("userId", id.ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id.ToString()));
            if (!string.IsNullOrEmpty(name)) claims.Add(new Claim(ClaimTypes.Name, name));
            if (!string.IsNullOrEmpty(surname)) claims.Add(new Claim(ClaimTypes.Surname, surname));
        }
        else if (role == Role.Club)
        {
            claims.Add(new Claim("clubId", id.ToString()));
            if (!string.IsNullOrEmpty(name)) claims.Add(new Claim("ClubName", name));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}