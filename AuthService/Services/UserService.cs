using AuthService.DTO;
using AuthService.EFCore;
using AuthService.Helper;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class UserService : IUserService, IDisposable
    {
        public ApplicationDbContext _context { get; set; }
        private IConfiguration _config;

        public UserService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public User GetUser(string username, string password)
        {
            return _context.User.Where(x => x.Name == username && x.Password == Encryption.EncryptString(_config.GetSection("EncryptionKey").Value, password)).FirstOrDefault();
        }

        public User GetUser(int nUserId)
        {
            return _context.User.Where(x => x.Id == nUserId).FirstOrDefault();
        }

        public User GetUserByEmailId(string emailId)
        {
            try
            {
                return _context.User.FirstOrDefault(x => x.Email == emailId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool RegisterUser(User user)
        {
            if (_context.User.Any(x => x.Email == user.Email))
            {
                return false;
            }

            user.Password = Encryption.EncryptString(_config.GetSection("EncryptionKey").Value, user.Password);
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
            _context.User.Update(user);
            _context.SaveChanges();
            return true;
        }

        public bool ChangeUserPassword(UserSetPassword objUserSetPassword)
        {
            User objCurrentUser = _context.User.AsNoTracking().Where(x => x.Id == objUserSetPassword.UserId).FirstOrDefault();
            if (objCurrentUser != null)
            {
                objCurrentUser.Password = Encryption.EncryptString(_config.GetSection("EncryptionKey").Value, objUserSetPassword.Password);
                _context.User.Update(objCurrentUser);
                _context.SaveChanges();
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
