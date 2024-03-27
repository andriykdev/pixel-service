using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StorageService.Services;

namespace StorageService;

public class MessageConsumerWorker : BackgroundService
{
    private readonly MessageHandler _messageHandler;
    private readonly string _queueName;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageConsumerWorker(IConfiguration configuration, MessageHandler messageHandler)
    {
        _messageHandler = messageHandler;

        _queueName = configuration["RabbitMQ:QueueName"] ?? throw new ArgumentException("Queue name is not defined");

        var hostName = configuration["RabbitMQ:Host"] ?? throw new ArgumentException("RabbitMq Host name is not defined");
        var userName = configuration["RabbitMQ:Username"] ?? throw new ArgumentException("RabbitMq User name is not defined");
        var password = configuration["RabbitMQ:Password"] ?? throw new ArgumentException("RabbitMq Password is not defined");

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            
            var handled = _messageHandler.Handle(message);
            
            // positively acknowledge a single delivery, if log was saved
            if (handled) _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        stoppingToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}