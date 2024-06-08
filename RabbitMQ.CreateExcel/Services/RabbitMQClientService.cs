using RabbitMQ.Client;

namespace RabbitMQ.CreateExcel.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _chanel;
        public static string ExchangeName = "ExcelDirectExchange";
        public static string RoutingExcel = "excel-route-file";
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
            _chanel.ExchangeDeclare(ExchangeName, type: "direct", true, false);
            _chanel.QueueDeclare(QueueNAme, true, false, false, null);

            _chanel.QueueBind(exchange: ExchangeName, queue: QueueNAme, routingKey: RoutingExcel);

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

