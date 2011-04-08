using System;
using System.Threading;
using RabbitMQ.Client;

namespace Mike.MQShootout
{
    public class Rabbit : IMessageSender<byte[]>, IMessageReceiver<byte[]>, IDisposable
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel sender;
        private IModel receiver;

        private const string queueName = "mySenderQueue";
        private const string exchangeName = "myExchange";
        private const string routingKey = "myRoutingKey";

        private Thread serverThread;

        public Rabbit()
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            connection = connectionFactory.CreateConnection();

            sender = connection.CreateModel();
            sender.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            sender.QueueDeclare(queueName, false, false, false, null);
            sender.QueueBind(queueName, exchangeName, routingKey, null);
        }

        public void Send(byte[] message)
        {
            sender.BasicPublish(exchangeName, routingKey, null, message);
        }

        public void ReceiveMessage(Action<byte[]> messageReceiver)
        {
            serverThread = new Thread(() =>
            {
                receiver = connection.CreateModel();
                receiver.QueueDeclare(queueName, false, false, false, null);

                var consumer = new QueueingBasicConsumer(receiver);
                receiver.BasicConsume(queueName, true, consumer);

                while (true)
                {
                    // TODO: consume messages here.
                }
            });
            serverThread.Start();
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (disposed) return;

            sender.Close();

            connection.Close();
            connection.Dispose();

            disposed = true;
        }
    }
}