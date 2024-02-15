using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ_Consumer_Exchange
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new("amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst");

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct); //1. Adım: Publisherdaki ile birebir aynı tanımlıyoruz.

            // 2.Adım: Publisher tarafından routing key'de bulunan değerdeki kuyruga gönderilen mesajları kendi olusturudugumuz kuyruga yönlendirerek
            // tüketmemiz gerekmektedir. Bunun için öncelikle kuyruk olusturulmalıdır.
            var queueName = channel.QueueDeclare().QueueName;


            // 3. Adım: 
            channel.QueueBind(queue: queueName, exchange: "direct-exchange-example", routingKey: "direct-queue-example");
            // üstteki routuingKey değerine gönderilmiş mesajları "queueName" adlı kuyruga göndermiş oluyoruz.


            //Queue'dan Mesaj Okuma
            EventingBasicConsumer consumer = new(channel);
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            consumer.Received += (sender, e) =>
            {
                string Message = Encoding.UTF8.GetString(e.Body.Span);
                Console.WriteLine(Message);

            };

            Console.Read();
        }
    }
}
