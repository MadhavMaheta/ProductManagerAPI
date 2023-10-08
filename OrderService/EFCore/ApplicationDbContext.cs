using OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderService.EFCore
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors


        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        #endregion

        #region Properties

        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItems> OrderItems { get; set; }

        #endregion

    }
}
