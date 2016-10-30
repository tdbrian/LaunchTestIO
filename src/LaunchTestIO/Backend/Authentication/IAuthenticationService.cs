using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;

namespace LaunchTestIO.Backend.Authentication
{
    public interface IAuthenticationService
    {
        Task ConfirmEmail(string userId, string code);
        Task ForgotPassword(string email);
        Task<IdentityUser> Login(string password, bool rememberMe, string email);
        Task LogOff();
        Task<IEnumerable<IdentityError>> Register(string email, string password);
        Task<IEnumerable<IdentityError>> ResetPassword(string email, string token, string password);
        Task SendAuthCode(bool rememberMe, string authProviderType);
        Task VerifyCode(string provider, bool rememberMe, string code, bool rememberBrowser);
    }
}