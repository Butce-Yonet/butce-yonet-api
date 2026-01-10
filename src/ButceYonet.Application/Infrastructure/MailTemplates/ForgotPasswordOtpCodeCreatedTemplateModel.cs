namespace ButceYonet.Application.Infrastructure.MailTemplates;

public class ForgotPasswordOtpCodeCreatedTemplateModel
{
    public string Email { get; set; }
    public string OtpCode { get; set; }
    public int Year { get; set; }
}