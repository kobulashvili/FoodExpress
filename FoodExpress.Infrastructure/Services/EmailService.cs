using System.Net;
using System.Net.Mail;
using FoodExpress.Domain.Interfaces;

namespace FoodExpress.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;

    public EmailService()
    {
        _smtpHost = "smtp.gmail.com";
        _smtpPort = 587;

        _senderEmail = "kobulakobula.13@gmail.com";
        _senderPassword = "xymv qxlq nbzp oygz";
    }

    public async Task SendOrderConfirmationAsync(
        string recipientEmail,
        string subject,
        string body)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException(
                "Recipient email is required.");

        try
        {
            using var message = new MailMessage();

            message.From = new MailAddress(_senderEmail);
            message.To.Add(recipientEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            using var smtp = new SmtpClient(
                _smtpHost,
                _smtpPort);

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(
                _senderEmail,
                _senderPassword);

            await smtp.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            throw new Exception(
                "Failed to send email.",
                ex);
        }
    }

    public async Task SendVerificationCodeAsync(
        string recipientEmail,
        string code)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException(
                "Recipient email is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Verification code is required.");

        try
        {
            using var message = new MailMessage();

            message.From = new MailAddress(_senderEmail);
            message.To.Add(recipientEmail);

            message.Subject =
                "FoodExpress Email Verification";

            message.Body =
                $"Your FoodExpress verification code is: {code}";

            message.IsBodyHtml = false;

            using var smtp = new SmtpClient(
                _smtpHost,
                _smtpPort);

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(
                _senderEmail,
                _senderPassword);

            await smtp.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            throw new Exception(
                "Failed to send verification email.",
                ex);
        }
    }
}