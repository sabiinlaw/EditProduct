using System.Collections.Generic;

namespace EditProductApp
{
    public class UserRepository
    {
        private static UserRepository repository = new UserRepository();
        private List<User> users = new List<User>();

        public static UserRepository GetRepository()
        {
            return repository;
        }

        public List<User> GetAllUsers()
        {
            return users;
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }
    }
}