using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaunchTestIO.Backend.Users
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool Deleted { get; set; }
        public bool IsAdmin { get; set; }

        public User(string firstName, string lastName, string emailAddress, bool isAdmin = false, string customerNumber = null)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            IsAdmin = isAdmin;
            Deleted = false;
            CustomerNumber = customerNumber;
        }
    }
}
