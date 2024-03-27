using Messages;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace PixelService.Services;

public class RabbitMqPublisher : IPublisher
{
    private readonly ConnectionFactory _factory;
    private readonly string _queueName;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _queueName = configuration["RabbitMQ:QueueName"] ?? throw new ArgumentException("Queue name is not defined");

        var hostName = configuration["RabbitMQ:Host"] ?? throw new ArgumentException("RabbitMq Host name is not defined");
        var userName = configuration["RabbitMQ:Username"] ?? throw new ArgumentException("RabbitMq User name is not defined");
        var password = configuration["RabbitMQ:Password"] ?? throw new ArgumentException("RabbitMq Password is not defined");

        _factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };
    }

    public void Send(VisitInfo visitInfo)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(visitInfo));

        channel.BasicPublish(exchange: string.Empty,
            routingKey: _queueName,
            basicProperties: null,
            body: body);
    }
}