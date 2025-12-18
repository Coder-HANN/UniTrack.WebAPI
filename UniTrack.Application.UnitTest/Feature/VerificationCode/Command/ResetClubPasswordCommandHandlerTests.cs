using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Enums;
using Xunit;

public class ResetClubPasswordCommandHandlerTests
{
    private readonly Mock<IClubRepository> _clubRepository = new();
    private readonly Mock<IVerificationCodeService> _codeService = new();
    private readonly Mock<IPasswordHasher<UniTrack.Domain.Entities.Club>> _passwordHasher = new();
    private readonly Mock<ILocalizationService> _localization = new();

    private ResetClubPasswordCommandHandler CreateHandler()
        => new(_clubRepository.Object, _codeService.Object, _passwordHasher.Object, _localization.Object);

    [Fact]
    public async Task Handle_InvalidCode_ReturnsFail()
    {
        _codeService.Setup(x =>
            x.ValidateCode(It.IsAny<string>(), It.IsAny<string>(), VerificationType.PasswordReset))
            .Returns(false);

        var handler = CreateHandler();

        var result = await handler.Handle(new ResetClubPasswordCommand
        {
            Email = "club@mail.com",
            Code = "1234",
            NewPassword = "newpass"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesClubPassword()
    {
        var club = new UniTrack.Domain.Entities.Club { PresidentMail = "club@mail.com" };

        _codeService.Setup(x =>
            x.ValidateCode(club.PresidentMail, "1234", VerificationType.PasswordReset))
            .Returns(true);

        _clubRepository.Setup(x => x.GetByEmailAsync(club.PresidentMail))
            .ReturnsAsync(club);

        _passwordHasher.Setup(x => x.HashPassword(club, "newpass"))
            .Returns("hashed");

        var handler = CreateHandler();

        var result = await handler.Handle(new ResetClubPasswordCommand
        {
            Email = club.PresidentMail,
            Code = "1234",
            NewPassword = "newpass"
        }, CancellationToken.None);

        club.Password.Should().Be("hashed");
        _clubRepository.Verify(x => x.UpdateAsync(club), Times.Once);
        _codeService.Verify(x =>
            x.RemoveCode(club.PresidentMail, VerificationType.PasswordReset), Times.Once);
    }
}
