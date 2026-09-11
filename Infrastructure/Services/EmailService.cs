using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetCodeAsync(string toEmail, string code)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "SendGrid API Key is missing.");
        }

        var fromEmail = _configuration["SendGrid:FromEmail"];
        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException(
                "SendGrid From Email is missing.");
        }
        var fromName = _configuration["SendGrid:FromName"];
        if (string.IsNullOrWhiteSpace(fromName))
        {
            throw new InvalidOperationException(
                "SendGrid From Name is missing.");
        }
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(
            from, to,
            "كود استعادة الباسورد - ElGhoul",
            $"كود استعادة الباسورد بتاعك هو: {code}\nصالح لمدة 10 دقائق بس.",
            $@"
        <div style='font-family: Arial; direction: rtl; text-align: right;'>
            <h2>استعادة الباسورد</h2>
            <p>كود استعادة الباسورد بتاعك هو:</p>
            <h1 style='color: #2196F3; letter-spacing: 5px;'>{code}</h1>
            <p>الكود صالح لمدة <strong>10 دقائق</strong> بس.</p>
        </div>"
        );

        await client.SendEmailAsync(msg);
    }
}