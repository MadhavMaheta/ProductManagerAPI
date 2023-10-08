using Newtonsoft.Json;
using ProductManagerAPI.EFCore;
using ProductManagerAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class RabbitMQConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                var connection = factory.CreateConnection();
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"Received message: {message}");

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                            var prod = dbContext.Product.Select(x => x).ToList();

                            List<ProductQuantity> listProducts = JsonConvert.DeserializeObject<List<ProductQuantity>>(message);
                            foreach (var item in listProducts) {
                               var product = prod.Where(x => x.Id == item.ProductId).FirstOrDefault();
                                product.Quantity -= item.Quantity;
                                dbContext.Update(product);
                            }
                            dbContext.SaveChanges();
                        }
                    };

                    channel.BasicConsume(queue: "decreaseQuantityQueue",
                                         autoAck: true,
                                         consumer: consumer);

                    await Task.Delay(1000, stoppingToken); // Add a delay to prevent busy-waiting
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                await Task.Delay(5000, stoppingToken); // Wait 5 seconds before retrying in case of an error
            }
        }
    }
}