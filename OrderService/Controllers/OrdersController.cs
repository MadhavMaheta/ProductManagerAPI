using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderService.Helper;
using OrderService.Models;
using OrderService.Services;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.Json;

namespace OrderService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public IUserOrderService _userOrderService { get; set; }
        public OrdersController(IUserOrderService userOrderService)
        {
            _userOrderService = userOrderService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult PlaceOrder(Order order)
        {
            bool isStockAvailable = IsStockAvailable(order.OrderItems.Select(x => new ProductQuantity { ProductId = x.ProductId, Quantity = x.Quantity }).ToList()).Result;

            if (!isStockAvailable) {
                throw new Exception("Stock is not available");
            }

            var factory = new ConnectionFactory() { HostName  = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "decreaseQuantityQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(order.OrderItems.Select(x => new ProductQuantity { ProductId = x.ProductId, Quantity = x.Quantity }).ToList());
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "decreaseQuantityQueue",
                                     basicProperties: null,
                                     body: body);
            }

            //_userOrderService.CreateOrder(order);
            return Ok();
        }

        private async Task<bool> IsStockAvailable(List<ProductQuantity> lstProducts)
        {
            using (var httpClient = new HttpClient())
            {
                HttpContent httpContent = GetContent<List<ProductQuantity>>(lstProducts);
                httpClient.DefaultRequestHeaders.Add("Authorization", this.HttpContext.Request.Headers["Authorization"].ToString());
                HttpResponseMessage result = await httpClient.PostAsync("https://localhost:7175/api" + "/Products/CheckProductStock",content:httpContent).ConfigureAwait(false);

                var stockAvailable = false;
                await result.Content.ReadAsStringAsync().ContinueWith(x =>
                {
                    stockAvailable = x.Result.ToLower().Equals("true") ? true : false;
                });

                return stockAvailable;
            }
        }

        public static HttpContent GetContent<T>(T model)
        {
            return new ObjectContent<T>(model, new JsonMediaTypeFormatter());
        }
    }
}
