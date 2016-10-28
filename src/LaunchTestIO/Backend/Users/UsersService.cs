using System;
using System.Collections.Generic;
using System.Linq;
using LaunchTestIO.Backend.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LaunchTestIO.Backend.Users
{
    public class UsersService: IUsersService
    {
        private readonly IUsersDatastore _userDatastore;
        private readonly AuthenticationService _authenticationService;
        private const string AdminEmail = "Admin@setup.test";
        private const string AdminPassword = "Pa55word!";

        public UsersService(IUsersDatastore userDatastore, AuthenticationService authenticationService)
        {
            _userDatastore = userDatastore;
            _authenticationService = authenticationService;
        }

        public async void PopulateDefaultAdmin()
        {
            if (await _userDatastore.HasDefaultAdminUser(AdminEmail)) return;
            var errors = await _authenticationService.Register(AdminEmail, AdminPassword) as IList<IdentityError>;
            if (errors == null || !errors.Any()) return;
            var identityErrors = errors.Select(e => e.Description).ToList();
            throw new Exception($"Unable to populate default user with the errors: {string.Join(", ", identityErrors)}");
        }
    }
}
