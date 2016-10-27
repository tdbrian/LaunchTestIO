using System.Threading.Tasks;
using LaunchTestIO.Config.Database;
using MongoDB.Driver;

namespace LaunchTestIO.Backend.Users
{
    public class UsersMongoDatastore : IUsersDatastore
    {
        private readonly ILaunchTestIoContext _dbContext;

        public UsersMongoDatastore(ILaunchTestIoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HasDefaultAdminUser()
        {
            return await _dbContext.Users.Find(x => x.IsAdmin && x.Deleted == false).AnyAsync();
        }

        public async Task<User> GetUser(string email)
        {
            return await _dbContext.Users.Find(x => x.EmailAddress.Equals(email) && x.Deleted == false).FirstAsync();
        }

        public async Task AddUser(User user)
        {
            await _dbContext.Users.InsertOneAsync(user);
        }
    }
}
