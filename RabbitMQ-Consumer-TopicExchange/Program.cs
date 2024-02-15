using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ_Consumer_TopicExchange
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

            Console.Write("Dinlenecek Topic formatını belirtin: ");
            string topic = Console.ReadLine();
            string queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue:queueName,exchange: "topic-exchange-example",routingKey:topic); // Olusturdugum exchange ile yukardaki kuyrugu bind (eşleştirme) edicem.

            EventingBasicConsumer consumer = new(channel);

            channel.BasicConsume(queue:queueName,autoAck:true,consumer);

            consumer.Received += (sender, e) =>
            {
                string Message = Encoding.UTF8.GetString(e.Body.Span);
                Console.WriteLine(Message);
            };

            Console.ReadLine();
        }
    }

    // #.Tegmen => Başı hiç önemli değil fakat sonu subay olsun.
    // #.Subay.# => Başı ve sonu hiç önemli değil.

}
