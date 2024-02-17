using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitSubscriber1

{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new()
            {
                Uri = new Uri("amqp://gedai:gedai@localhost:5672"),
                ClientProvidedName = "RabbitSubscriber1NET"
            };
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();

            string exchangeName = "DemoExchange";
            string queueName = "DemoQueue";
            string routingKey = "demo.rk";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Slow Message Received: {message} with id {args.BasicProperties.MessageId}");
                channel.BasicAck(args.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(queueName, false, consumer);

            Console.ReadLine();

            channel.Close();
            connection.Close();

        }
    }
}
