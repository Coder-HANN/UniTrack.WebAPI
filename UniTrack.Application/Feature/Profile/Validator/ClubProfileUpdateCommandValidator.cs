using FluentValidation;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Profile.Command;

public class ClubProfileUpdateCommandValidator
    : AbstractValidator<ClubProfileUpdateCommand>
{
    public ClubProfileUpdateCommandValidator(
        IClubRepository clubRepository,
        IUserRepository userRepository,
        ILocalizationService localizationService)
    {
        RuleFor(x => x)
            .MustAsync(async (command, _) =>
            {
                var clubs = await clubRepository.GetAllAsync();
                var users = await userRepository.GetAllAsync();

                bool mailExistsInClubs = clubs.Any(c =>
                    c.PresidentMail == command.PresidentMail ||
                    c.ContectEmail == command.ContectEmail);

                bool mailExistsInUsers = users.Any(u =>
                    u.Email == command.PresidentMail ||
                    u.Email == command.ContectEmail);

                return !mailExistsInClubs && !mailExistsInUsers;
            })
            .WithMessage(localizationService
                .Get(ValidationKeys.EmailAlreadyUsed).Result);
    }
}
