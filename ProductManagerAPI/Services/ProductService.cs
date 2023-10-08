using ProductManagerAPI.EFCore;
using ProductManagerAPI.Models;

namespace ProductManagerAPI.Services
{
    public class ProductService : IProductService, IDisposable
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<Product> GetProducts()
        {
            return _context.Product.ToList();
        }
    }
}
