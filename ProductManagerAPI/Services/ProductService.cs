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

        public List<Product> GetFilteredProducts(Filter filter)
        {
            return _context.Product.Where(x=> filter.categories.Count > 0 ? filter.categories.Any(y => y == x.CategoryId) : true).ToList();
        }
    }
}
