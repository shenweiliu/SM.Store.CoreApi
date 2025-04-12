using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Store.Api.Common
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        //Task<IEnumerable<User>> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Account",
                Username = "BasicAuthAccount",
                Password = "Hb87j#G34asYm&f%Op"
            }
        };

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so return user details without password
            return user.WithoutPassword();
        }

        //public async Task<IEnumerable<User>> GetAll()
        //{
        //    return await Task.Run(() => _users.WithoutPasswords());
        //}
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
