using RabbitMQ.Client;
using System.Text;

namespace RabbitPublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new()
            {
                Uri = new Uri("amqp://gedai:gedai@localhost:5672"),
                ClientProvidedName = "RabbitPublisherNET"
            };
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();

            string exchangeName = "DemoExchange";
            string queueName = "DemoQueue";
            string routingKey = "demo.rk";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey);

            var limit = 60;
            for (int i = 0; i < limit; i++)
            {
                var message = $"#{i.ToString().PadLeft(limit.ToString().Length, '0')} - Hello YouTube!";
                byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
                var id = Guid.NewGuid();
                var props = channel.CreateBasicProperties();
                props.MessageId = id.ToString();
                channel.BasicPublish(exchangeName, routingKey, props, messageBodyBytes);
                Console.WriteLine($"Sending \"{message}\" with id {props.MessageId}");
                Thread.Sleep(1000);
            }

            channel.Close();
            connection.Close();
        }
    }
}
