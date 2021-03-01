using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AppModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace WorkerApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        const string QueueName = "WorkerQueue";

        public Worker(ILogger<Worker> logger, ConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(QueueName);
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (bc, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation($"Processing msg: '{message}'.");
                try
                {
                    var workItem = JsonSerializer.Deserialize<WorkItem>(message);
                    _logger.LogInformation($"Processing item {workItem.Name}.");
                    
                    byte[] buffer = new byte[1024];
                    var r = new Random();
                    // Do some useless work (computing hashes over and over)
                    byte[] combinedHash = null;
                    for (int i = 0; i < workItem.AmountOfWork; i++)
                    {
                        r.NextBytes(buffer);
                        
                        using (var sha = SHA256.Create())
                        {
                            var tempBuffer = sha.ComputeHash(buffer);
                            if (combinedHash == null)
                                combinedHash = tempBuffer;
                            else
                            {
                                
                                for (int j = 0; j < combinedHash.Length; j++)
                                {
                                    combinedHash[j] = (byte)(((int)combinedHash[j] + (int)tempBuffer[j]) % 256);
                                }
                            }
                            
                        }
                    }

                    _logger.LogInformation($"Item #{workItem.Name} complete. Result: " + Convert.ToBase64String(combinedHash));
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (JsonException)
                {
                    _logger.LogError($"JSON Parse Error: '{message}'.");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (AlreadyClosedException)
                {
                    _logger.LogInformation("RabbitMQ is closed!");
                }
                catch (Exception e)
                {
                    _logger.LogError(default, e, e.Message);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;

        }
    }
}
