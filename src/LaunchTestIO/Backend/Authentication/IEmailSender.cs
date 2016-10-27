using System.Threading.Tasks;

namespace LaunchTestIO.Backend.Authentication
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
