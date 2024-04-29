using AuthService.DTO;
using AuthService.Models;

namespace AuthService.Services
{
    public interface IUserService
    {
        public User GetUser(string username, string password);
        public bool RegisterUser(User user);
        public bool UpdateUser(User user);
        public bool ChangeUserPassword(UserSetPassword objUserSetPassword);
        User GetUser(int nUserId);
        User GetUserByEmailId(string emailId);
        List<User> GetUserList();
    }
}
