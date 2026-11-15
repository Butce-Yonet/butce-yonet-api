namespace ButceYonet.Application.Infrastructure.MailTemplates;

public class ForgotPasswordOtpCodeCreatedTemplateModel : LocalizedMailTemplateModel
{
    public string Email { get; set; }
    public string OtpCode { get; set; }
}