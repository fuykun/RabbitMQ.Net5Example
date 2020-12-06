using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

const string hostName = "localhost";
const string queueName = "mail";

// RabbitMQ bağlantısı oluşturuyoruz.
var factory = new ConnectionFactory() { HostName = hostName };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    Console.WriteLine($"'{hostName}' üzerinde ki RabbitMQ server'a bağlanıldı.");

    // 'mail' isimli kuyrukla işlem yapacağımızı belirtiyoruz.
    channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

    // göndereceğimiz mail nesnesini tanımlıyoruz ve json'a çeviriyoruz.
    var email = new Mail(DateTime.Now, "mailaddress@mail.com", "Title", "Content");
    string message = JsonConvert.SerializeObject(email);

    // kuyruğa gönderilecek mesajı byte'a çevirmemiz gerekiyor.
    var body = Encoding.UTF8.GetBytes(message);

    Console.WriteLine($"'{queueName}' adlı kuyruğa mesaj gönderiliyor...");

    // mesajı RabbitMQ'dya ekliyoruz
    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

    Console.WriteLine("Mail başarıyla kuyruğa eklendi.");
}
Console.ReadLine();

record Mail(DateTime mailTime, string MailAddress, string Title, string Content);