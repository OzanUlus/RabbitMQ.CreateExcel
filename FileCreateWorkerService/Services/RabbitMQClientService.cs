using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCreateWorkerService.Services
{
    public class RabbitMQClientService :IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _chanel;
       
        public static string QueueNAme = "queue-excel-file";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

        }
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_chanel is { IsOpen: true })
            {
                return _chanel;
            }
            _chanel = _connection.CreateModel();
          

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");


            return _chanel;

        }

        public void Dispose()
        {
            _chanel?.Close();
            _chanel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");

        }
    }
}
