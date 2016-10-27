using LaunchTestIO.Backend.Customers;
using LaunchTestIO.Backend.Users;
using MongoDB.Driver;

namespace LaunchTestIO.Config
{
    public interface ILaunchTestIoContext
    {
        IMongoCollection<Customer> Customers { get; }
        IMongoCollection<User> Users { get; }
    }
}