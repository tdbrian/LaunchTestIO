using System.Threading.Tasks;

namespace LaunchTestIO.Backend.Users
{
    public interface IUsersDatastore
    {
        Task<User> GetUser(string email);
        Task<bool> HasDefaultAdminUser(string email);
        Task AddUser(User user);
    }
}