# RABBİTMQ - EXAMPLES


# RabbitMQ Example Projects

Bu repo, RabbitMQ ile ilgili farklı örnek çalışmalarımı içermektedir. Her bir klasör, farklı bir RabbitMQ kullanımı veya tasarım modelini göstermektedir. Aşağıda, her bir klasörün içeriği hakkında kısa açıklamalar bulabilirsiniz.

## İçerik

- [RabbitMQ-Consumer-Exchange](#rabbitmq-consumer-exchange)
- [RabbitMQ-Consumer-TopicExchange](#rabbitmq-consumer-topicexchange)
- [RabbitMQ-MessageDesign](#rabbitmq-messagedesign)
- [RabbitMQ-Publisher-Exchange](#rabbitmq-publisher-exchange)
- [RabbitMQ-MassTransit](#rabbitmq-masstransit)

## RabbitMQ-Consumer-Exchange

Bu klasör, RabbitMQ'da Exchange kullanarak bir mesaj tüketicisinin nasıl oluşturulacağını göstermektedir. Exchange, mesajların yönlendirildiği ve kuyruğa yerleştirildiği bir bileşendir.

- **Örnekler:**
  - Temel Consumer-Exchange yapılandırması
  - Farklı Exchange türlerinin kullanımı (Direct, Fanout, Topic, Headers)

## RabbitMQ-Consumer-TopicExchange

Bu klasörde, Topic Exchange kullanarak bir mesaj tüketicisinin nasıl oluşturulacağını görebilirsiniz. Topic Exchange, mesajları routing key'e göre yönlendirmede kullanılır.

- **Örnekler:**
  - Topic Exchange ile mesaj yönlendirme
  - Routing key desenleri kullanarak filtreleme

## RabbitMQ-MessageDesign

Bu klasör, RabbitMQ'da mesaj tasarımı ve yapılandırması konusundaki örnekleri içerir. Mesaj tasarımı, mesajların nasıl oluşturulacağı ve yönlendirileceği hakkında bilgi sağlar.

- **Örnekler:**
  - Mesaj formatları ve içerik tasarımı
  - Mesaj özellikleri ve başlık bilgileri

## RabbitMQ-Publisher-Exchange

RabbitMQ'da Exchange kullanarak bir mesaj yayımlayıcısının (publisher) nasıl oluşturulacağını gösteren örnekleri içerir. Publisher, mesajları Exchange'e gönderir.

- **Örnekler:**
  - Temel Publisher-Exchange yapılandırması
  - Farklı Exchange türleri ile mesaj yayımlama

## RabbitMQ-MassTransit

Bu klasör, MassTransit kullanarak RabbitMQ ile entegrasyonun nasıl yapılacağını gösteren örnekleri içerir. MassTransit, mesajlaşma altyapılarıyla çalışmak için kullanılan bir .NET kütüphanesidir.

- **Örnekler:**
  - MassTransit ile RabbitMQ yapılandırması
  - Mesaj gönderme ve tüketme işlemleri


