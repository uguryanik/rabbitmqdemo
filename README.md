# RabbitMQ Demo Project

Bu proje, RabbitMQ üzerinde mesajları üreten (RabbitProducer) ve tüketen (RabbitListener) iki .NET Core uygulaması içerir. Bu uygulamalar Docker üzerinde çalışır ve birbirleriyle RabbitMQ aracılığıyla iletişim kurarlar. Ayrıca, RabbitListener MongoDB'ye bağlanır ve orada veri saklar.

## Proje Yapısı

Ana dizin altında bulunan projeler:

- `RabbitProducer`: RabbitMQ'ya mesaj gönderen .NET Core uygulaması.
- `RabbitListener`: RabbitMQ'dan mesaj alan ve MongoDB'ye saklayan .NET Core uygulaması.
- `Models`: Uygulamalar arasında paylaşılan veri modellerini içerir.
- `Core`: Veritabanı ve diğer servislerle etkileşim için kullanılan ortak kodları içerir.

## Başlarken

Projeyi ayağa kaldırmak için Docker ve Docker Compose'a ihtiyacınız olacaktır. Docker'ı [resmi web sitesinden](https://www.docker.com/products/docker-desktop) indirebilirsiniz.

### Adım 1: Projeyi klonlayın

```bash
git clone https://github.com/uguryanik/rabbitmqdemo.git
cd rabbitmqdemo
```

### Adım 2: Docker Compose ile servisleri başlatın

```bash
docker-compose up
```

### Adım 3: Testlerin Çalıştırılması

Her bir servis için (RabbitProducer, RabbitListener) unit testleri mevcuttur. Testleri çalıştırmak için önce test projelerine gitmeniz ve ardından dotnet test komutunu kullanmanız gerekir:

```bash
cd tests/RabbitProducer.Tests
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

```bash
cd tests/RabbitListener.Tests
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Proje içindeki bileşenler:

1. rabbitmq: Bu servis, RabbitMQ mesajlaşma sunucusunu başlatıyor. 

2. rabbitproducer: Bu servis, RabbitMQ kuyruğuna mesaj gönderen bir uygulamayı başlatır. 

3. rabbitlistener: Bu servis, RabbitMQ'den mesajları dinleyen ve işleyen bir uygulamayı başlatır. 

4. mongodb: Bu servis, MongoDB NoSQL veritabanı sunucusunu başlatır. 

5. mongo-express: Bu servis, MongoDB veritabanınızı yönetmek ve görselleştirmek için bir web tabanlı araç olan Mongo Express'i başlatır.

6. setup: Bu servis, Elastic Stack'ın kurulumunu ve yapılandırmasını sağlar. Gerekli ortam değişkenlerini tanımlar ve ayrıca özel roller ve yetenekler oluşturur.

7. elasticsearch: Bu servis, Elasticsearch sunucusunu başlatır.

8. kibana: Bu servis, Elasticsearch veri setlerini görselleştirmek için Kibana'yı başlatır. 

servislerin kullandığı portlar docker-compose içersinde düzenlenebilir. Mevcut hali ile servislerin dinlediği portlar:

- mongodb 27017:27017 (username: root, password: somepassword)
- mongoexpress 27081:8081
- rabbitmq 15672:15672 (username: guest, password: guest)
            5672:5672
- elasticsearch 9200:9200 (username: elastic, password: somepassword)
                9300:9300
- kibana        5601:5601


