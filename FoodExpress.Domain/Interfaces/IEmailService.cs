namespace FoodExpress.Domain.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(
        string recipientEmail,
        string subject,
        string body);

    Task SendVerificationCodeAsync(
        string recipientEmail,
        string code);
}