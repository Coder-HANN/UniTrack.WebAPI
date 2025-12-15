using Microsoft.AspNetCore.Http;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Domain.Enums;

public class CurrentUserServices : ICurrentUserServices
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserServices(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Guid? CurrentUser()
    {
        if (httpContextAccessor.HttpContext?.Items["userId"] is Guid id)
            return id;

        var userIdClaim = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            return userId;

        return null;
    }

    public Guid? CurrentClub()
    {
        if (httpContextAccessor.HttpContext?.Items["clubId"] is Guid id)
            return id;

        var clubIdClaim = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "clubId");
        if (clubIdClaim != null && Guid.TryParse(clubIdClaim.Value, out Guid clubId))
            return clubId;

        return null;
    }

    public Role? Role()
    {
        if (httpContextAccessor.HttpContext?.Items["role"] is Role role)
            return role;

        var roleClaim = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "role");
        if (roleClaim != null && Enum.TryParse<Role>(roleClaim.Value, out var parsedRole))
            return parsedRole;

        return null;
    }
}
