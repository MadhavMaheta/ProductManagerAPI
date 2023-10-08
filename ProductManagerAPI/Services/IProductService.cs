using ProductManagerAPI.Models;

namespace ProductManagerAPI.Services
{
    public interface IProductService
    {
        List<Product> GetProducts();
    }
}
