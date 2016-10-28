using Microsoft.AspNetCore.Identity.MongoDB;

namespace LaunchTestIO.Backend.Users
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerNumber { get; set; }
        public bool Deleted { get; set; }
        public bool IsAdmin { get; set; }

        public User(string email)
        {
            base.Email = email;
            base.UserName = email;
        }

        public User(string firstName, string lastName, string email, bool isAdmin = false, string customerNumber = null)
        {
            base.Email = email;
            base.UserName = email;
            FirstName = firstName;
            LastName = lastName;
            IsAdmin = isAdmin;
            Deleted = false;
            CustomerNumber = customerNumber;
        }
    }
}
