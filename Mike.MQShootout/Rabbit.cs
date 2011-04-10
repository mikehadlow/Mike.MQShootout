using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mike.MQShootout
{
    public class Rabbit : IMessageSender<byte[]>, IMessageReceiver<byte[]>, IDisposable
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly IConnection senderConnection;
        private IConnection receiverConnection;
        private readonly IModel sender;
        private IModel receiver;

        private const string queueName = "myQueue";

        private Thread serverThread;

        public Rabbit()
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            senderConnection = connectionFactory.CreateConnection();

            sender = senderConnection.CreateModel();
            sender.QueueDeclare(queueName, false, false, false, null);
        }

        public void Send(byte[] message)
        {
            sender.BasicPublish("", queueName, null, message);
        }

        public void ReceiveMessage(Action<byte[]> messageReceiver)
        {
            serverThread = new Thread(() =>
            {
                receiverConnection = connectionFactory.CreateConnection();
                receiver = receiverConnection.CreateModel();
                receiver.QueueDeclare(queueName, false, false, false, null);

                var consumer = new QueueingBasicConsumer(receiver);
                receiver.BasicConsume(queueName, true, consumer);

                while (true)
                {
                    var message = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    messageReceiver(message.Body);
                }
            });
            serverThread.Start();
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (disposed) return;

            sender.Close();
            sender.Dispose();

            if (serverThread != null)
            {
                serverThread.Abort();
            }

            if (receiver != null)
            {
                receiver.Close();
                receiver.Dispose();
            }

            senderConnection.Close();
            senderConnection.Dispose();

            if (receiverConnection != null)
            {
                receiverConnection.Close();
                receiverConnection.Dispose();
            }

            disposed = true;
        }
    }
}