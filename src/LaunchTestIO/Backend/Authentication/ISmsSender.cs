using System.Threading.Tasks;

namespace LaunchTestIO.Backend.Authentication
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
