using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ_Consumer
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

            //Queue Oluşturma
            channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);
            //Consumer'da bağlantı publisher ile birebir yapıda olmalıdır.

            //Queue'dan Mesaj Okuma
            EventingBasicConsumer consumer = new(channel);
            channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer); // hangi kuyrugu dinleyeceğimizi ilk parametreye yazıyoruz.
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); // her kuyruga eşit sayıda göndermemizi sağlıyor.

            //Gelen mesajı yakalayabilmesi için receive etmemiz gerekiyor.
            consumer.Received += (sender, e) =>
            {
                //Kuyruga gelen mesajın işlendiği yerdir.
                // e.body bize kuyruktaki mesajın body'sini bütünsel getirir.
                // e.body.span veya e.body.ToArray() =>Kuyruktaki mesajın byte verisini getirecektir.

                Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));

                channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);

                //channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);


                //var consumerTag = channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer);
                //channel.BasicCancel(consumerTag);

                //channel.BasicReject(deliveryTag: 3, requeue: true);
            };

            Console.Read();

        }


        #region Message Acknowledgement

        /*  -- BasicAck -- 
         * AutoAck parametresini mesaj onaylama süreçlerinde kullanıyoruz. Mesaj onaylama durumu olsun istiyorsan bu değeri "false" yapıyoruz.
         * Böykece RabbitMQ'da varsayılan olarak mesajların kuyruktan silinme davranısı değişecek ve consumer'dan onay bekleyecek.
         * Consumer, mesajın başarıyla işlendiğine dair uyarıyı 'channel.BasicAck' metoduyla gerçekleştirir.
         * multiple parametresi: birden fazla mesaja dair onay bildirisi göderir. True verilirse DeliveryTag değerine sahip olan bundan önceki mesajlarda
         * işlendiğine dair bilgilendirir. False ise sadece bu mesaj için onay bildirisi verir.
         */


        /* -- BasicNack ile işlenmeyen mesajları geri gönderme --
         * İşlenmeyen yani başarısız dönen mesajların bildirisi için bunu kullanıp RabbitMQ'ya bilgi vererek mesajı tekrar işletebiliriz.
         * Requeue parametresi -> bu consumer tarafından mesajın işlenip işlenmeycegi ifade edilen bu mesajın tekrar kuyruga eklenip eklenmemesinin kararını verir.
         * True değeri verilirse mesaj işlenmek üzere kuyruga eklenir. false değerinde ise eklenmemek üzere silinir.
         * Sadece bu mesajın işlenmeyecegine dair RabbitMQ'ya bilgi verilmiş olunacaktır.
         */


        /* -- basicCancel ile bir kuyruktaki tüm mesajların işlenmesini reddetme --
         * Bu metotla consumerTag değerine karsılık gelen queue'daki tüm mesajlar reddedilerek işlenmez.
        */


        /* -- BasicReject ile tek bir mesajın işlenmesini reddetme --
         * Bu metotla kuyrukta bulunan mesajların belirli olanların consumer tarafından işlenmesini istemediğimiz durumlarda kullanılırız.
         * BasicCancel kısmında direkt kuyrugu reddederken burada sadece ilgili mesajları reddediyoruz. **
        */

        // ASLINDA BU KONUDA CONSUMER'LARIN SIKINTI YAŞAMA DURUMUNDA MESAJLARIN KAYBOLMAMA GARANTİSİNİN NASIL SAĞLANACAGINI ÖĞRENDİK.
        #endregion




        #region Fair Dispatch

        // RabbitMQ'da bu ayarı yapmazsak consumer'larımıza eşit şekilde mesaj dağılımı olmayacaktır ve eşitsizlik olacaktır. 
        // BasicQos metodu ile mesajların işleme hızını ve teslimat sırasını belirleyebiliriz.

        /* KULLANIMI
         * channel.BasicQos(perefetchSize:0,prefetchCount:1,global:false);
         * 1 parametre bir consumer tarafından alınabilcek max mesaj boyutunu byte turunden belirler. 0, sınırsız demektir.
         * 2. parametre bir consumer tarafından aynı anda işleme alınabilcek mesaj sayısını belirler.
         * 3. parametre konfigurasyonun tüm consumerlar için mi yoksa sadece çağrı yapılan consumer için mi geçerli olacagını belirler.
         */

        #endregion
    }
}
