using MassTransit;
using MassTransit_Consumer.Consumers;

string rabbitMQUri = "amqps://mbilpnst:wmN5rZF9k8Ke7Sgv9duSt85Ynbh5d9Ln@shark.rmq.cloudamqp.com/mbilpnst";
string queueName = "example";

var bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);

    factory.ReceiveEndpoint(queueName, endpoint => endpoint.Consumer<ExampleMessageConsumer>());
});

await bus.StartAsync();
Console.ReadLine();