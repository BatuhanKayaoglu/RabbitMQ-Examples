using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ;
using RabbitMQ.Client;

namespace RabbitMQ_GY
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Bağlantı Oluşturma
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new("amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst");

            // Bağlantıyı Aktifleştirme ve Kanal Açma
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();


            //Queue Oluşturma
            channel.QueueDeclare(queue: "example-queue", exclusive: false,durable:true);

            // Kuyruk Güvenliği konusu
            IBasicProperties properties=channel.CreateBasicProperties();    
            properties.Persistent = true;

            // Queue'ya mesaj gönderme
            // RabbitMQ kuyruğa atacağı mesajları byte türünden kabul ediyor. Haliyle mesajları byte haline dönüştürmeliyiz.
            for (int i = 0; i < 80; i++) 
            {
                Task.Delay(500).Wait();    
                byte[] message = Encoding.UTF8.GetBytes("Merhaba: "+i);
                channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message,basicProperties:properties); // yani direct exch kullanıyoruz şu an. 
            }

            Console.ReadLine(); 

        }


        #region Message Durability

        // RabbitMQ'da (sunucuda) normal şartlarda bir kapanam durumu söz konusu oldugunda tüm kuyruklar ve mesajlar silinecektir.
        // Bu çalışma; kuyruk ve mesaj açısından kalıcı olarak işaretleme yapmamızı gerektirmektedir.

        // QueueDeclare kısmında 'durable:true' olarak ayarlanması gerekiyor kuyrugu kalıcı hale getirebilmek için. (mesaj için konfigurasyon)
        // IBasicProperties metoduyla olan işlemi de mesaj konfigurasyonu için eklememiz gerekiyor.
        //BasicPublish kısmında ise basicProperties: özelliğini eklememiz gerekiyor.
        // Consumer kısmında da QueueDeclare kısmını 'durable:true' yapmayı unutmamalıyız.

        #endregion



    }
}
