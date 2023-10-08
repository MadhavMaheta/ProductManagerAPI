using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductManagerAPI.EFCore;
using ProductManagerAPI.Models;
using ProductManagerAPI.Services;
using StackExchange.Redis;

namespace ProductManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private IProductService _productService;

        public ProductsController(IProductService productService, ApplicationDbContext context, IWebHostEnvironment environment, IConnectionMultiplexer connectionMultiplexer)
        {
            _productService = productService;
            _context = context;
            _environment = environment;
            _connectionMultiplexer = connectionMultiplexer;
        }

        // GET: api/Products
        [HttpGet]
        [Helper.Authorize("Admin")]
        public List<Product> GetProducts()
        {
            return  _productService.GetProducts();
        }

        [HttpGet]
        public List<Product> GetHomePageProducts()
        {
            var db = _connectionMultiplexer.GetDatabase();
            var value  =db.StringGet("HomePage");

            if (value.IsNull)
            {
                List<Product> list = _context.Product.ToList();
                db.StringSet("HomePage", JsonConvert.SerializeObject(list),TimeSpan.FromDays(1));
                return list;
            }
            else {
                return JsonConvert.DeserializeObject<List<Product>>(value.ToString());
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> GetProductImage(long id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null || string.IsNullOrEmpty(product.ImageName))
                return null;

            Byte[] byteImage = System.IO.File.ReadAllBytes(_environment.WebRootPath + "\\Images\\" + product.ImageName);
            return byteImage;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProductImage()
        {
            var product = _context.Product.Select(x => x).ToList();

            foreach (var item in product)
            {
                if (!string.IsNullOrEmpty(item.ImageName))
                    item.ImageData = System.IO.File.ReadAllBytes(_environment.WebRootPath + "\\Images\\" + item.ImageName);
            }
            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPost("{id}"), DisableRequestSizeLimit]
        [Authorize]
        public bool AddImage(long id)
        {
            try
            {
                var product = _context.Product.Find(id);
                if (product == null)
                {
                    return false;
                }

                var file = Request.Form.Files[0];
                string fileName = file.FileName;
                using (FileStream filestream = System.IO.File.Create(_environment.WebRootPath + "\\Images\\" + fileName))
                {
                    file.CopyTo(filestream);
                    filestream.Flush();
                }

                product.ImageName = fileName;
                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public bool CheckProductStock(List<ProductQuantity> lstProducts) {

            foreach(var prod in lstProducts)
            {
                int quantity = _context.Product.Where(x => x.Id == prod.ProductId).Select(x=>x.Quantity).FirstOrDefault();

                if(quantity < prod.Quantity)
                    return false;
            }

            return true;  
        }

        [Authorize]
        private bool ProductExists(long id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
