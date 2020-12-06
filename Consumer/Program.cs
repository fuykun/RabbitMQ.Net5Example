using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;


// RabbitMQ bağlantısı oluşturuyoruz.
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{

    // 'mail' kuyruğuna erişiyoruz.
    channel.QueueDeclare(queue: "mail", durable: false, exclusive: false, autoDelete: false, arguments: null);

    // 'mail' kuyruğundaki verileri alıyoruz.
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var email = JsonConvert.DeserializeObject<Mail>(message);
        Console.WriteLine($"\nTarih: {email.mailTime}\nBaşlık: {email.Title}\nMail Adresi: {email.MailAddress}\nMesaj: {email.Content}");
    };
    channel.BasicConsume(queue: "mail", autoAck: true, consumer: consumer);

    Console.WriteLine(" Mesaj başarılı ile kuyrukdan alındı.");
}
Console.ReadLine();


record Mail(DateTime mailTime, string MailAddress, string Title, string Content);