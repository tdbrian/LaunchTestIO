using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LaunchTestIO.Backend.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string ProviderPhone = "Phone";
        private const string ProviderEmail = "Email";
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;

        public AuthenticationService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILoggerFactory loggerFactory, 
            ISmsSender smsSender, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AuthenticationService>();
            _smsSender = smsSender;
            _emailSender = emailSender;
        }

        public async Task<IdentityUser> Login(string password, bool rememberMe, string email)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(1, "User logged in.");
                return await _userManager.GetUserAsync(ClaimsPrincipal.Current);
            }

            if (result.RequiresTwoFactor)
            {
                await SendAuthCode(rememberMe, ProviderEmail);
                throw new HubException("Two factor authentication required.");
            }

            if (result.IsLockedOut)
            {
                var msg = "User account locked out.";
                _logger.LogWarning(2, msg);
                throw new HubException(msg);
            }

            _logger.LogInformation(1, "Invalid login attempt.");
            throw new HubException("Invalid Login Attempt");
        }

        public async Task SendAuthCode(bool rememberMe, string authProviderType)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) throw new Exception("No user found for 2FA authentication");

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, authProviderType);
            if (string.IsNullOrWhiteSpace(code)) throw new Exception("Invalid 2FA code was generated");

            var message = "Your two factor authentication code is: " + code;

            switch (authProviderType)
            {
                case ProviderEmail:
                    await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "LaunchTest.io Two Factor Authentication Code", message);
                    break;
                case ProviderPhone:
                    await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
                    break;
                default:
                    throw new Exception("Unknown 2FA provider type passed.");
            }
        }

        public async Task VerifyCode(string provider, bool rememberMe, string code, bool rememberBrowser)
        {
            var result = await _signInManager.TwoFactorSignInAsync(provider, code, rememberMe, rememberBrowser);
            if (result.Succeeded) return;
            if (!result.IsLockedOut) throw new Exception("Invalid Code");
            _logger.LogWarning(7, "User account locked out.");
            throw new Exception("Sorry, you've been locked out for too many attempts.");
        }

        public async Task ForgotPassword(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            
            // Don't reveal that the user does not exist or is not confirmed
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) return;

            // Send an email with this link
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{user.Id} {code}";
            await _emailSender.SendEmailAsync(email, "Reset Password", 
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
        }

        public async Task ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) throw new Exception("Invalid input");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(!result.Succeeded) throw new Exception("Error confirming email.");
        }

        public async Task<IEnumerable<IdentityError>> ResetPassword(string email, string token, string password)
        {
            var user = await _userManager.FindByNameAsync(email);

            // Dont reveal if the user in not found
            if (user == null) return new List<IdentityError>();

            var result = await _userManager.ResetPasswordAsync(user, token, password);
            return result.Errors;
        }

        public async Task LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
        }

        public async Task<IEnumerable<IdentityError>> Register(string email, string password)
        {
            var user = new IdentityUser {Email = email, UserName = email};
            var result = await _userManager.CreateAsync(user, password);

            if (result.Errors.Any()) return result.Errors;
            
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = $"{user.Id} {code}";
            await _emailSender.SendEmailAsync(email, "Confirm your account",
                $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation(3, "User created a new account with password.");
            return result.Errors;
        }
    }
}
