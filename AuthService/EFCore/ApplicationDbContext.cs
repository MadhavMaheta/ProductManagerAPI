using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.EFCore
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors


        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        #endregion

        #region Properties

        public virtual DbSet<User> User { get; set; }

        #endregion

    }
}
