using LaunchTestIO.Backend.Customers;
using LaunchTestIO.Backend.Users;
using MongoDB.Driver;

namespace LaunchTestIO.Config
{
    public class LaunchTestIoContext: ILaunchTestIoContext
    {
        public IMongoDatabase Database;

        public LaunchTestIoContext()
        {
            var client = new MongoClient("mongodb://localhost:27017/launch-test");
            Database = client.GetDatabase("launch-test");
        }

        public IMongoCollection<User> Users => Database.GetCollection<User>("users");

        public IMongoCollection<Customer> Customers => Database.GetCollection<Customer>("customers");
    }
}
