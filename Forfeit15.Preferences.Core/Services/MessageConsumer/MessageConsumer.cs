using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Forfeit15.Preferences.Core.Services.MessageConsumer;

public class MessageConsumer : IHostedService
{
    private readonly string? _connectionString;
    private readonly string _queueName;

    public MessageConsumer(string? connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received message: {0}", message);

            // Do something with the message, e.g. process it asynchronously
            await ProcessMessageAsync(message);

            // Acknowledge the message to remove it from the queue
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: _queueName,
            autoAck: false,
            consumer: consumer);

        while (!cancellationToken.IsCancellationRequested)
        {
            // Wait for new messages
            await Task.Delay(1000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop the message consumer loop here
        return Task.CompletedTask;
    }

    private Task ProcessMessageAsync(string message)
    {
        // TODO: Implement message processing logic here
        Console.WriteLine("PROCESSED");
        return Task.CompletedTask;
    }
}
