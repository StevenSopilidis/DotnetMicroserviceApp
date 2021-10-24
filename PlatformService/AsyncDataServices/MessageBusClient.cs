using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration config)
        {
            _config = config;
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQHOST"],
                Port= int.Parse(_config["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("Connected to MessageBus");                
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Could not connect to Event Bus: {ex.Message}");                
            }
        } 
        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if(_connection.IsOpen)
            {
                Console.WriteLine("RabbitMQ connection opened, sending message");
                SendMessage(message);
            }else 
            {
                Console.WriteLine("RabbitMQ connection closed, sending message");
            }
        }

        private void SendMessage(string message)
        {
            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(message)
            );
        }

        public void Dispose()
        {
            Console.WriteLine("Message bus client disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs args)
        {
            Console.WriteLine("Event bus connection shutdown");
        }
    }
}