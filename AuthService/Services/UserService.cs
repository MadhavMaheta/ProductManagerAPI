using AuthService.DTO;
using AuthService.EFCore;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthService.Services
{
    public class UserService : IUserService, IDisposable
    {
        public ApplicationDbContext _context { get; set; }

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public User GetUser(string username, string password)
        {
            return _context.User.Where(x => x.Name == username && x.Password == password).FirstOrDefault();
        }

        public bool RegisterUser(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateUser(User user)
        {
            User objCurrentUser = _context.User.AsNoTracking().Where(x => x.Id == user.Id).FirstOrDefault();
            if (objCurrentUser == null)
            {
                throw new Exception("User data not found");
            }
            objCurrentUser.Name = user.Name;
            objCurrentUser.Email = user.Email;
            objCurrentUser.Password = user.Password;
            _context.User.Update(user);
            _context.SaveChanges();
            return true;
        }

        public User GetUser(int nUserId)
        {
            return _context.User.Where(x => x.Id == nUserId).FirstOrDefault();
        }

        public bool ChangeUserPassword(UserSetPassword objUserSetPassword)
        {
            User objCurrentUser = _context.User.AsNoTracking().Where(x => x.Id == objUserSetPassword.UserId).FirstOrDefault();
            if (objCurrentUser != null)
            {
                objCurrentUser.Password = objCurrentUser.Password;
                return true;
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public List<User> GetUserList()
        {
            return _context.User.ToList();
        }
    }
}
