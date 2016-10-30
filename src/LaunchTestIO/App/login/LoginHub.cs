using System.Threading.Tasks;
using LaunchTestIO.Backend.Authentication;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.SignalR;

namespace LaunchTestIO.App.login
{
    public class LoginHub : Hub<ILoginHubClient>
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginHub(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public Task<IdentityUser> AttemptLogin(string email, string password, bool rememberMe)
        {
            return _authenticationService.Login(password, rememberMe, email);
        }

        public void PushVersion(string version)
        {
            Clients.All.PushVersionToClient(version);
        }
    }

    public interface ILoginHubClient
    {
        string PushVersionToClient(string version);
    }
}
