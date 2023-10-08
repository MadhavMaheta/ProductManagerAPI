using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ProductManagerAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace ProductManagerAPI.Services
{
    public class RabbitMQBackgroundConsumerService
    {
        public RabbitMQBackgroundConsumerService()
        {
        }

        public void ExecuteAsync()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            //channel.QueueDeclare("decreaseQuantityQueue");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };
            channel.BasicConsume(queue: "decreaseQuantityQueue", autoAck: true, consumer: consumer);

            //var factory = new ConnectionFactory() { HostName = "localhost" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "decreaseQuantityQueue",
            //                         durable: true,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    var consumer = new EventingBasicConsumer(channel);
            //    consumer.Received += (model, ea) =>
            //    {
            //        byte[] body = ea.Body.ToArray();
            //        string message = Encoding.UTF8.GetString(body);
            //        List<ProductQuantity> lstQty = JsonConvert.DeserializeObject<List<ProductQuantity>>(message);
            //        Console.WriteLine(message);
            //    };

            //    channel.BasicConsume("decreaseQuantityQueue", true,consumer);
            //}
        }
    }
}
