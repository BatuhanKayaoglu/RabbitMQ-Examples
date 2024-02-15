using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


#region P2P (Point-to-Point) Tasarımı
// Bir publisher ilgili mesajı direkt bir kuyruga gönderir ve bu mesaj kuyrugu işleyen bir consumer tarafından dinleyerek tüketilir. 
// Bir mesajın bir consumer tarafından işlenmesi gerektiği senaryolarda kullanılır.

string queueName = "example-p2p-queue";

channel.QueueDeclare(
    queue: queueName,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: queueName,
    autoAck: false,
    consumer: consumer
    );

consumer.Received += (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};

#endregion



#region Publish/Subscribe (Pub/Sub) Tasarımı

// Bu tasarımda publisher mesajı bir exchange'e gönderir ve böylece mesaj bu exchange'e bind edilmiş olan tüm kuyruklara yönlendirilir.
// Bu tasarım, bir mesajın birçok tüketici tarafından işlenmesi gerektiği durumlarada kullanışlıdır.

string exchangeName = "example-pub-sub-exchange";

// Önceden açılmış exchange' declare oluyoruz.
channel.ExchangeDeclare(
    exchange: exchangeName,
    type: ExchangeType.Fanout
    );

// Farklı kuyruk isimlerinden aynı exchange'e bağlanmış olacaklar. Hepsi exchangeName içersinde yer alan exchange'e bind edilmiş olacaklar birden fazla console çalıştırdıgımızda
string queueName2 = channel.QueueDeclare().QueueName;

// Kuyruga bind ettik.
channel.QueueBind(
    queue: queueName2,
    exchange: exchangeName,
    routingKey: string.Empty
    );

channel.BasicQos(
    prefetchCount:1,
    prefetchSize:1,
    global:false
    );


EventingBasicConsumer consumer2= new(channel);
channel.BasicConsume(
    queue: queueName,
    autoAck: false,
    consumer: consumer2
    );

consumer.Received += (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};


#endregion



#region Work Queue(İş Kuyrugu) Tasarımı

string exchangeName2 = "example-work-exchange";

channel.QueueDeclare(
    queue: exchangeName2,
    durable: false,
    exclusive: false,
    autoDelete: false
    );

EventingBasicConsumer consumer3 = new(channel);
channel.BasicConsume(
    queue: exchangeName2,
    autoAck: true, // Her mesajın yalnızca bir consumer tarafından tüketilebilmesi için autoAck özelligi true verilmiş. (Her tarafa eşit dagılacagı için)
    consumer: consumer3
    );


channel.BasicQos(
    prefetchCount: 1, // Her consumer 1 mesaj işlemesi için
    prefetchSize: 0, // totalde sınırsız mesaj işleyebilirler.
    global: false
    );

consumer.Received += (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};

#endregion



#region Request/Response Tasarımı

string queueName3 = "example-request-response-queue";

channel.QueueDeclare(
    queue: queueName3,
    durable: false,
    exclusive: false,
    autoDelete: false
    );


EventingBasicConsumer consumer4 = new(channel);
channel.BasicConsume(
    queue: queueName3,
    autoAck: false,
    consumer: consumer4
    );

consumer.Received += (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));

    byte[] responseMessage = Encoding.UTF8.GetBytes("İşlem Tamamlandı!");

    // Gelen mesajın korelasyon değerini alıp, gönderilecek mesajın property'sine işliyoruz.
    IBasicProperties basicProperties = e.BasicProperties;
    IBasicProperties replyProperties = channel.CreateBasicProperties();
    replyProperties.CorrelationId = basicProperties.CorrelationId;
    channel.BasicPublish(
        exchange:string.Empty,
        // Gelen mesajdaki RepyTo property'sinde bulunan kuyruk adını buradaki routingKey'e vererek response amacıyla kullanıyoruz.
        routingKey:basicProperties.ReplyTo,
        basicProperties:replyProperties,
        body:responseMessage
        );
};

#endregion

Console.ReadLine();