using Microsoft.Extensions.Caching.Memory;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Domain.Enums;

namespace UniTrack.Infrastructure.Services // Sizin namespace'iniz farklı olabilir
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IMemoryCache cache;
        private readonly IMailService mailService;

        public VerificationCodeService(IMemoryCache cache, IMailService mailService)
        {
            this.cache = cache;
            this.mailService = mailService;
        }

        public async Task GenerateAndSendCodeAsync(string email, VerificationType type)
        {
            var code = new Random().Next(100000, 999999).ToString();
            var key = $"VERIFY_{type}_{email}";
            cache.Set(key, code, TimeSpan.FromMinutes(3));

            var subject = type switch
            {
                VerificationType.ClubRegistration => "Kulüp Hesabınızı Doğrulayın — Öğrencity",
                VerificationType.UserRegistration => "Hesabınızı Doğrulayın — Öğrencity",
                VerificationType.PasswordReset => "Şifre Sıfırlama — Öğrencity",
                VerificationType.EmailChange => "E-posta Değişikliği — Öğrencity",
                _ => "Doğrulama Kodu — Öğrencity"
            };

            var body = BuildVerificationEmailHtml(code, type);
            await mailService.SendMailAsync(email, subject, body);
        }

        private string BuildVerificationEmailHtml(string code, VerificationType type)
        {
            var title = type switch
            {
                VerificationType.ClubRegistration => "Kulüp hesabınızı doğrulayın",
                VerificationType.UserRegistration => "Hesabınızı doğrulayın",
                VerificationType.PasswordReset => "Şifrenizi sıfırlayın",
                VerificationType.EmailChange => "E-posta adresinizi doğrulayın",
                _ => "Doğrulama kodunuz"
            };

            var description = type switch
            {
                VerificationType.ClubRegistration => "Öğrencity'e hoş geldiniz! Kulüp hesabınızı aktifleştirmek için aşağıdaki doğrulama kodunu girin.",
                VerificationType.UserRegistration => "Öğrencity'e hoş geldiniz! Hesabınızı aktifleştirmek için aşağıdaki doğrulama kodunu girin.",
                VerificationType.PasswordReset => "Şifrenizi sıfırlamak için aşağıdaki kodu kullanın. Bu kodu siz talep etmediyseniz güvende olabilirsiniz.",
                VerificationType.EmailChange => "E-posta adresinizi güncellemek için aşağıdaki doğrulama kodunu girin.",
                _ => "Aşağıdaki kodu ilgili alana girin."
            };

            var formattedCode = $"{code[..3]} {code[3..]}";

            return $@"
                <!DOCTYPE html>
                <html lang=""tr"">
                <head>
                  <meta charset=""UTF-8"" />
                  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                  <title>{title}</title>
                </head>
                <body style=""margin:0;padding:0;background:#f8f7f4;font-family:'Helvetica Neue',Arial,sans-serif;"">

                  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8f7f4;padding:40px 16px;"">
                    <tr>
                      <td align=""center"">
                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width:480px;"">

                          <!-- LOGO -->
                          <tr>
                            <td align=""center"" style=""padding-bottom:28px;"">
                              <span style=""font-size:22px;font-weight:700;color:#2563eb;letter-spacing:-0.5px;"">
                                Öğrencity
                              </span>
                            </td>
                          </tr>

                          <!-- KART -->
                          <tr>
                            <td style=""background:#ffffff;border:1px solid #e8e4dd;border-radius:20px;padding:40px 36px;"">

                              <!-- İKON -->
                              <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                  <td align=""center"" style=""padding-bottom:24px;"">
                                    <div style=""width:56px;height:56px;background:#eff6ff;border:1px solid #bfdbfe;border-radius:14px;display:inline-block;text-align:center;line-height:56px;font-size:26px;"">
                                      🔐
                                    </div>
                                  </td>
                                </tr>
                              </table>

                              <!-- BAŞLIK -->
                              <h1 style=""margin:0 0 12px;font-size:22px;font-weight:700;color:#1a1815;text-align:center;letter-spacing:-0.5px;"">
                                {title}
                              </h1>

                              <!-- AÇIKLAMA -->
                              <p style=""margin:0 0 32px;font-size:14px;color:#5a5550;line-height:1.7;text-align:center;"">
                                {description}
                              </p>

                              <!-- KOD KUTUSU -->
                              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:32px;"">
                                <tr>
                                  <td align=""center"">
                                    <div style=""display:inline-block;background:#f0f7ff;border:2px dashed #93c5fd;border-radius:14px;padding:20px 40px;"">
                                      <span style=""font-size:36px;font-weight:800;color:#2563eb;letter-spacing:8px;font-family:'Courier New',monospace;"">
                                        {formattedCode}
                                      </span>
                                    </div>
                                  </td>
                                </tr>
                              </table>

                              <!-- UYARI -->
                              <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                  <td style=""background:#fefce8;border:1px solid #fde68a;border-radius:10px;padding:12px 16px;"">
                                    <p style=""margin:0;font-size:12.5px;color:#92400e;text-align:center;line-height:1.6;"">
                                      ⏱ Bu kod <strong>3 dakika</strong> geçerlidir. Kimseyle paylaşmayın.
                                    </p>
                                  </td>
                                </tr>
                              </table>

                            </td>
                          </tr>

                          <!-- ALT BİLGİ -->
                          <tr>
                            <td align=""center"" style=""padding-top:24px;"">
                              <p style=""margin:0;font-size:11.5px;color:#a09a94;line-height:1.6;"">
                                Bu e-postayı siz talep etmediyseniz görmezden gelebilirsiniz.<br/>
                                © 2025 Öğrencity · Tüm hakları saklıdır.
                              </p>
                            </td>
                          </tr>

                        </table>
                      </td>
                    </tr>
                  </table>

                </body>
                </html>";
        }
        public bool ValidateCode(string email, string code, VerificationType type)
        {
            var key = $"VERIFY_{type}_{email}";
            if (cache.TryGetValue(key, out string cachedCode))
            {
                return cachedCode == code;
            }
            return false;
        }

        public void RemoveCode(string email, VerificationType type)
        {
            var key = $"VERIFY_{type}_{email}";
            cache.Remove(key);
        }
    }
}