using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderService.Helper;
using OrderService.Models;
using OrderService.Services;
using Polly;
using Polly.CircuitBreaker;
using RabbitMQ.Client;
using System.Net.Http.Formatting;
using System.Text;

namespace OrderService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public IUserOrderService _userOrderService { get; set; }
        private IModel _channel;
        private IConnection _connection;
        public OrdersController(IUserOrderService userOrderService)
        {
            _userOrderService = userOrderService;
        }

        [HttpGet]
        public IActionResult TextCB()
        {
            try
            {
                List<ProductQuantity> newList = new List<ProductQuantity>();
                newList.Add(new ProductQuantity { ProductId = 1, Quantity = 1 });
                bool isStockAvailable = IsStockAvailable(newList);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost]
        public IActionResult PlaceOrder(Order order)
        {
            bool isStockAvailable = IsStockAvailable(order.OrderItems.Select(x => new ProductQuantity { ProductId = x.ProductId, Quantity = x.Quantity }).ToList());

            if (!isStockAvailable) {
                throw new Exception("Stock is not available");
            }

            string message = JsonConvert.SerializeObject(order.OrderItems.Select(x => new ProductQuantity { ProductId = x.ProductId, Quantity = x.Quantity }).ToList());
            var body = Encoding.UTF8.GetBytes(message);

            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            string queueName = "myQueue";  // Replace with your queue name
            _channel.QueueDeclare(queue: "decreaseQuantityQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.BasicPublish(exchange: "",
                                 routingKey: "decreaseQuantityQueue",
                                 basicProperties: null,
                                 body: body);

            //_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            //var factory = new ConnectionFactory() { HostName  = "localhost" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "decreaseQuantityQueue",
            //                         durable: true,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    string message = JsonConvert.SerializeObject(order.OrderItems.Select(x => new ProductQuantity { ProductId = x.ProductId, Quantity = x.Quantity }).ToList());
            //    var body = Encoding.UTF8.GetBytes(message);

            //    channel.BasicPublish(exchange: "",
            //                         routingKey: "decreaseQuantityQueue",
            //                         basicProperties: null,
            //                         body: body);
            //}

            _userOrderService.CreateOrder(order);
            return Ok();
        }

        private bool IsStockAvailable(List<ProductQuantity> lstProducts)
        {
            if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
            {
                throw new Exception("Service Unavailable");
            }

            using (var httpClient = new HttpClient())
            {
                HttpContent httpContent = GetContent<List<ProductQuantity>>(lstProducts);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Abc");
                //HttpResponseMessage result = await httpClient.PostAsync("https://localhost:7175/api" + "/Products/CheckProductStock",content:httpContent).ConfigureAwait(false);

                HttpResponseMessage result = CircuitBreakerPolicy.ExecuteAsync(() =>
                httpClient.PostAsync("https://localhost:7175/api" + "/Products/CheckProductStock", content: httpContent)).Result;

                var stockAvailable = false;
                result.Content.ReadAsStringAsync().ContinueWith(x =>
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

        public static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy = 
            Policy.HandleResult<HttpResponseMessage>(message => (int) message.StatusCode != 200)
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
    }
}
