using Application.Interfaces;

namespace Infrastructure.Email;

public class EmailProvider : IEmailProvider
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        return Task.CompletedTask;
    }
}
