using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ;
using RabbitMQ.Client;

namespace RabbitMQ_Exchange
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new("amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst");

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange:"direct-exchange-example",type:ExchangeType.Direct);
            // Type 'direct' oldugundan rabbitmq routing key'e bakacaktır. Ona karsılık gelen exchange hangisiyse ona gönderecek. 

            while (true) {
                Console.Write("Mesaj: ");
                string message=Console.ReadLine();
                byte[] byteMessage=Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "direct-exchange-example",routingKey:"direct-queue-example",body:byteMessage); // routingKey ismi önemli değil.
            }

            Console.Read();
        }
    }
}
