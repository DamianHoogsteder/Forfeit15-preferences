using System.Text;
using System.Text.Json;
using Forfeit15.Preferences.Core.Services.Preferences;
using Forfeit15.Preferences.Core.Services.Preferences.Implementations;
using Forfeit15.Preferences.Core.Services.Preferences.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Forfeit15.Preferences.Core.Services.MessageConsumer;

public class MessageConsumer : BackgroundService
{
    private readonly string? _connectionString;
    private readonly string _queueName;
    private readonly IServiceProvider _serviceProvider;

    public MessageConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _connectionString = "amqp://lxwoxkvq:tkjW4M8P7fOj-jM7Yu9I4-yTiKYj-yIK@cow.rmq2.cloudamqp.com/lxwoxkvq";
        _queueName = "forfeit15";
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
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

    private async Task<Task> ProcessMessageAsync(string message)
    {
        var update = JsonSerializer.Deserialize<UpdateMessageVM>(message);

        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var scopedProcessingService =
                scope.ServiceProvider.GetRequiredService<IPreferenceService>();

            await scopedProcessingService.PushNotificationAsync(update, new CancellationToken());
        }

        return Task.CompletedTask;
    }
}