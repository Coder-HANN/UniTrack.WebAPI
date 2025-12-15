using Microsoft.EntityFrameworkCore;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using UniTrack.Persistence.Context;

namespace UniTrack.WebAPI.Extensions
{
    public static class AdminExtencsions
    {
        public static IServiceCollection AddAdminServices(this IServiceCollection services)
        {

           
            return services;
        }
    }
}
