using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;


// RabbitMQ bağlantısı oluşturuyoruz.
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    // 'mail' isimli kuyrukla işlem yapacağımızı belirtiyoruz.
    channel.QueueDeclare(queue: "mail", durable: false, exclusive: false, autoDelete: false, arguments: null);

    // göndereceğimiz mail nesnesini tanımlıyoruz ve json'a çeviriyoruz.
    var email = new Mail(DateTime.Now, "mailaddress@mail.com", "Title", "Content");
    string message = JsonConvert.SerializeObject(email);

    // kuyruğa gönderilecek mesajı byte'a çevirmemiz gerekiyor.
    var body = Encoding.UTF8.GetBytes(message);

    // mesajı RabbitMQ'dya ekliyoruz
    channel.BasicPublish(exchange: "", routingKey: "mail", basicProperties: null, body: body);

    Console.WriteLine("Mail başarıyla kuyruğa eklendi.");
}
Console.ReadLine();

record Mail(DateTime mailTime, string MailAddress, string Title, string Content);