using Microsoft.EntityFrameworkCore;
using ProductManagerAPI.Models;

namespace ProductManagerAPI.EFCore
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors


        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        #endregion

        #region Properties

        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<User> User { get; set; }

        #endregion

    }
}
