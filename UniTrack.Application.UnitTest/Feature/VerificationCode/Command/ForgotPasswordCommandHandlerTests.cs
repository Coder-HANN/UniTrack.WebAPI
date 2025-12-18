using FluentAssertions;
using Moq;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;
using Xunit;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IClubRepository> _clubRepository = new();
    private readonly Mock<IVerificationCodeService> _codeService = new();
    private readonly Mock<ILocalizationService> _localization = new();

    private ForgotPasswordCommandHandler CreateHandler()
        => new(_userRepository.Object, _codeService.Object, _clubRepository.Object, _localization.Object);

    [Fact]
    public async Task Handle_UserOrClubNotFound_ReturnsFail()
    {
        _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);

        var handler = CreateHandler();

        var result = await handler.Handle(new ForgotPasswordCommand
        {
            Email = "test@mail.com"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ValidUser_SendsCode()
    {
        _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());

        _clubRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UniTrack.Domain.Entities.Club());

        var handler = CreateHandler();

        var result = await handler.Handle(new ForgotPasswordCommand
        {
            Email = "test@mail.com"
        }, CancellationToken.None);

        _codeService.Verify(x =>
            x.GenerateAndSendCodeAsync("test@mail.com", VerificationType.PasswordReset), Times.Once);

        result.IsSuccess.Should().BeTrue();
    }
}
