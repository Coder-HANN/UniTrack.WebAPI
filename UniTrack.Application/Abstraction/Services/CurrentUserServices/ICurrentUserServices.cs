using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Services.CurrentUserServices
{
    public interface ICurrentUserServices
    {
        public Guid? CurrentUser();
        public Guid? CurrentClub();
        public Role? Role();

    }
}
