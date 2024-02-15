using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst");

// Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();



#region P2P (Point-to-Point) Tasarımı

// Bir publisher ilgili mesajı direkt bir kuyruga gönderir ve bu mesaj kuyrugu işleyen bir consumer tarafından dinleyerek tüketilir. 
// Bir mesajın bir consumer tarafından işlenmesi gerektiği senaryolarda kullanılır.

string queueName = "example-p2p-queue";

channel.QueueDeclare(
    queue:queueName,
    durable:false,
    exclusive:false,    
    autoDelete:false
    );

byte[] message = Encoding.UTF8.GetBytes("Merhaba");
channel.BasicPublish(
    exchange:string.Empty,
    routingKey:queueName,
    body:message
    );

#endregion



#region Publish/Subscribe (Pub/Sub) Tasarımı

// Bu tasarımda publisher mesajı bir exchange'e gönderir ve böylece mesaj bu exchange'e bind edilmiş olan tüm kuyruklara yönlendirilir.
// Bu tasarım, bir mesajın birçok tüketici tarafından işlenmesi gerektiği durumlarada kullanışlıdır.
// Kuyruk adı(routingKey) gözetmeksizin bind olmuş tüm consumerlar istediğini alabiliyor.
string exchangeName = "example-pub-sub-exchange";

// Bir exchange alanı açıyoruz.
channel.ExchangeDeclare(
    exchange:exchangeName,
    type:ExchangeType.Fanout
    );

for (int i = 0; i < 100; i++){

byte[] body = Encoding.UTF8.GetBytes("Merhaba!!");

// Mesajımızı o exchange içersine pushluyoruz.
channel.BasicPublish(
    exchange:exchangeName,
    routingKey:string.Empty,
    body:body
    );
}

#endregion



#region Work Queue(İş Kuyrugu) Tasarımı

// Publisher tarafından yayınlanmış bir mesajın birden fazla consumer arasından yalnızca biri tarafından tüketilmesi amaçlanmaktadır.  
// Böylece mesajların işlenmesi tüm consumer'lar aynı iş yüküne ve eşit görev dagılımına sahip olacaktır.
// Mesajları işlendikten sonra sürekli silmemiz gerekiyor ki consumerlar eşit görev dagılımına sahip olabilsin.

string exchangeName2 = "example-work-exchange";

channel.QueueDeclare(
    queue: exchangeName2,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

IBasicProperties properties =channel.CreateBasicProperties();
properties.Persistent = true; // Yayınlanan mesaj kalıcı olarak gönderilmektedir.

for (int i = 0; i < 100; i++)
{
    await Task.Delay(300);
    byte[] body = Encoding.UTF8.GetBytes("Merhaba!!: "+i);

    channel.BasicPublish(
    exchange: string.Empty,
    routingKey: exchangeName2,
    body: body,
    basicProperties:properties  
    );
}

#endregion



#region Request/Response Tasarımı

// Bu tasarımda publisher bir request yapar gibi kuyruga mesaj gönderir ve bu mesajı tüketen consumer'dan sonuca dair başka kuyruktan bir response bekler.

string queueName3 = "example-request-response-queue";

channel.QueueDeclare(
    queue: queueName3,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

// Response'un dinleneceği queue tanımlıyoruz. () Consumerdan dönecek olan sonucu elde edeceğimiz kuryugun adını tanımlıyoruz.
string replyQueueName = channel.QueueDeclare().QueueName;

// Response sürecinde hangi request'e karsılık response'un yapılacagını ifade edecek olan korelasyonel değer olusturuluyor.
string correlationId=Guid.NewGuid().ToString(); // Gönderilen mesajı ifade eden bir korelasyon değeri olusturuyoruz.

#region Request Mesajını Olusturma ve gönderme Kısmı
IBasicProperties basicProperties = channel.CreateBasicProperties(); 

// Request korelasyon değeriyle eşleştiriliyor.
basicProperties.CorrelationId = correlationId;  // Göndereceğimiz mesajın korelasyon değerini taşıyor.

// response yapılacak queue 'ReplyTo' property'sine atanıyor.
basicProperties.ReplyTo = replyQueueName; // Göndereceğimiz mesaja karsılık dönen response'un hangi kuyruga gönderilcegini ifade ediyor.

byte[] body2 = Encoding.UTF8.GetBytes("Request Message");

channel.BasicPublish(
exchange: string.Empty,
routingKey: queueName3,
body: body2,
basicProperties: basicProperties
);
#endregion

#region Response kuyrugunu dinleme

EventingBasicConsumer consumer = new(channel);

channel.BasicConsume(
    queue:queueName3,
    autoAck:true,
    consumer:consumer   
    );

consumer.Received += (sender, e) =>
{
    if (e.BasicProperties.CorrelationId == correlationId)
    {
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    }
};

#endregion

#endregion


Console.ReadLine();