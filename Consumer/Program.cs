using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

const string hostName = "localhost";
const string queueName = "mail";

// RabbitMQ bağlantısı oluşturuyoruz.
var factory = new ConnectionFactory() { HostName = hostName };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    Console.WriteLine($"'{hostName}' üzerinde ki RabbitMQ server'a bağlanıldı.");

    // 'mail' kuyruğuna erişiyoruz.
    channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

    // 'mail' kuyruğundaki verileri alıyoruz.
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var email = JsonConvert.DeserializeObject<Mail>(message);
        Console.WriteLine($"\nTarih: {email.mailTime}\nBaşlık: {email.Title}\nMail Adresi: {email.MailAddress}\nMesaj: {email.Content}");
    };
    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

    Console.WriteLine("Kuyruktaki mesajlar başarılı ile alındı.");
}
Console.ReadLine();


record Mail(DateTime mailTime, string MailAddress, string Title, string Content);