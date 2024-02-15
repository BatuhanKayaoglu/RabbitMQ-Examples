using MassTransit;
using MassTransit_Shared.Messages;

string rabbitMQUri = "amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst";
string queueName = "example";

var bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});

ISendEndpoint sendEndPoint = await bus.GetSendEndpoint(new($"{rabbitMQUri}/{queueName}"));

Console.Write("Gönderilecek Mesaj:");
string message = Console.ReadLine();

await sendEndPoint.Send<IMessage>(new ExampleMessage
{
    Text = message
});

Console.ReadLine();

