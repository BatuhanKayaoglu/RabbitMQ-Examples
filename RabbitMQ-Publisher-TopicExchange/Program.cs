using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Publisher_TopicExchange
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Bağlantı Oluşturma
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new("amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst");

            // Bağlantıyı Aktifleştirme ve Kanal Açma
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "topic-exchange-example", type: ExchangeType.Topic);

            for (int i = 0; i < 100; i++)
            {
                Task.Delay(200);
                byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");
                Console.Write("Mesajın gönderilecegi Topic formatını belirtin: ");
                string topic = Console.ReadLine();
                channel.BasicPublish(exchange: "topic-exchange-example",routingKey:topic,body:message);

            }
            Console.Read(); 

        }
    }
}
