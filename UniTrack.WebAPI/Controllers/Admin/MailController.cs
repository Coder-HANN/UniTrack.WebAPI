using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Services.Mail;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] EmailConfirmationMessage message)
        {
            var subject = "Email Confirmation";
            var body = $"Please confirm your email using this token: {message.Token}";

            await _mailService.SendMailAsync(message.Email, subject, body);
            return Ok("Mail gönderildi.");
        }
    }
}
