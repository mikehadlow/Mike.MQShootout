using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;

namespace Mike.MQShootout
{
    public class ActiveMq : IMessageSender<byte[]>, IMessageReceiver<byte[]>, IDisposable
    {
        private readonly IConnection connection;
        private readonly ISession session;
        private readonly ITopic topic;
        private readonly IMessageProducer sender;
        private IMessageConsumer receiver;

        private const string brokerUri = "tcp://localhost:61616";

        public ActiveMq()
        {
            var connectionFactory = new NMSConnectionFactory(brokerUri);
            connection = connectionFactory.CreateConnection();
            connection.Start();
            session = connection.CreateSession();

            // need to create session
            topic = new ActiveMQTopic("testTopic");
            sender = session.CreateProducer(topic);
        }

        public void Send(byte[] message)
        {
            var bytesMessage = sender.CreateBytesMessage(message);
            sender.Send(bytesMessage);
        }

        public void ReceiveMessage(Action<byte[]> messageReceiver)
        {
            receiver = session.CreateConsumer(topic);
            receiver.Listener += message =>
            {
                var byteMessage = message as IBytesMessage;
                if (byteMessage == null)
                {
                    throw new ApplicationException("byteMessage was null");
                }
                messageReceiver(byteMessage.Content);
            };
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (disposed) return;
            
            sender.Close();
            sender.Dispose();
            
            if (receiver != null)
            {
                receiver.Close();
                receiver.Dispose();
            }

            session.Close();
            session.Dispose();

            connection.Stop();
            connection.Close();
            connection.Dispose();

            disposed = true;
        }
    }
}