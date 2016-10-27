namespace LaunchTestIO.Backend.Users
{
    public class UsersService: IUsersService
    {
        private readonly IUsersDatastore _userDatastore;
        private const string AdminName = "Admin";

        public UsersService(IUsersDatastore userDatastore)
        {
            _userDatastore = userDatastore;
        }

        public async void PopulateDefaultAdmin()
        {
            if (await _userDatastore.HasDefaultAdminUser()) return;
            var defaultAdminuser = new User(AdminName, AdminName, null, true);
            await _userDatastore.AddUser(defaultAdminuser);
        }
    }
}
