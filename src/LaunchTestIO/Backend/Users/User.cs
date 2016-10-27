using LaunchTestIO.Backend.Customers;

namespace LaunchTestIO.Backend.Users
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Customer Customer { get; set; }
    }
}
