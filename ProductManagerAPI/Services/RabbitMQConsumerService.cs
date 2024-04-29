using Newtonsoft.Json;
using ProductManagerAPI.EFCore;
using ProductManagerAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Channels;

public class RabbitMQConsumerService : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConsumerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitRabbitMQ();
    }

    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        //_channel = _connection.CreateModel();
        //_channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
        //_channel.QueueDeclare("demo.queue.log", false, false, false, null);
        //_channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
        //_channel.BasicQos(0, 1, false);

        //_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var channel = _connection.CreateModel())
        {
            channel.QueueDeclare(queue: "decreaseQuantityQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

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
                    foreach (var item in listProducts)
                    {
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

        }

        //stoppingToken.ThrowIfCancellationRequested();

        //var consumer = new EventingBasicConsumer(_channel);
        //consumer.Received += (ch, ea) =>
        //{
        //    var body = ea.Body.ToArray();
        //    var message = Encoding.UTF8.GetString(body);
        //    Console.WriteLine($"Received message: {message}");

        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //        var prod = dbContext.Product.Select(x => x).ToList();

        //        List<ProductQuantity> listProducts = JsonConvert.DeserializeObject<List<ProductQuantity>>(message);
        //        foreach (var item in listProducts)
        //        {
        //            var product = prod.Where(x => x.Id == item.ProductId).FirstOrDefault();
        //            product.Quantity -= item.Quantity;
        //            dbContext.Update(product);
        //        }
        //        dbContext.SaveChanges();
        //    }
        //    // handle the received message
        //    _channel.BasicAck(ea.DeliveryTag, false);
        //};

        //consumer.Shutdown += OnConsumerShutdown;
        //consumer.Registered += OnConsumerRegistered;
        //consumer.Unregistered += OnConsumerUnregistered;
        //consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        //_channel.BasicConsume("decreaseQuantityQueue", false, consumer);
        return Task.CompletedTask;
    }

    private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }

    private readonly IServiceProvider _serviceProvider;

    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    while (!stoppingToken.IsCancellationRequested)
    //    {
    //        try
    //        {
    //            var factory = new ConnectionFactory { HostName = "localhost" };
    //            var connection = factory.CreateConnection();
    //            using (var channel = connection.CreateModel())
    //            {
    //                var consumer = new EventingBasicConsumer(channel);
    //                consumer.Received += (model, ea) => 
    //                {
    //                    var body = ea.Body.ToArray();
    //                    var message = Encoding.UTF8.GetString(body);
    //                    Console.WriteLine($"Received message: {message}");

    //                    using (var scope = _serviceProvider.CreateScope())
    //                    {
    //                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //                        var prod = dbContext.Product.Select(x => x).ToList();

    //                        List<ProductQuantity> listProducts = JsonConvert.DeserializeObject<List<ProductQuantity>>(message);
    //                        foreach (var item in listProducts) {
    //                           var product = prod.Where(x => x.Id == item.ProductId).FirstOrDefault();
    //                            product.Quantity -= item.Quantity;
    //                            dbContext.Update(product);
    //                        }
    //                        dbContext.SaveChanges();
    //                    }
    //                    channel.BasicAck(ea.DeliveryTag, false);
    //                };

    //                channel.BasicConsume(queue: "decreaseQuantityQueue",
    //                                     autoAck: false,
    //                                     consumer: consumer);

    //                await Task.Delay(1000, stoppingToken); // Add a delay to prevent busy-waiting
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error occurred: {ex.Message}");
    //            await Task.Delay(5000, stoppingToken); // Wait 5 seconds before retrying in case of an error
    //        }
    //    }
    //}
}