using System;
using System.Messaging;

namespace Mike.MQShootout
{

    public class Msmq<T> : IMessageSender<T>, IMessageReceiver<T>
        where T : class 
    {
        const string queueName = @".\Private$\TestQueue";
        private readonly MessageQueue sendQueue;
        private readonly MessageQueue receiveQueue;

        public Msmq()
        {
            if (!MessageQueue.Exists(queueName))
            {
                MessageQueue.Create(queueName);
            }

            var formatter = new XmlMessageFormatter(new Type[] {typeof (T)});

            sendQueue = new MessageQueue(queueName) {Formatter = formatter};
            receiveQueue = new MessageQueue(queueName) { Formatter = formatter };
        }

        public void Send(T message)
        {
            var envelope = new Message(message) {Recoverable = false};
            sendQueue.Send(envelope);
        }

        public void ReceiveMessage(Action<T> messageReceiver)
        {
            receiveQueue.ReceiveCompleted += (sender, args) =>
            {
                try
                {
                    var result = args.Message.Body as T;
                    messageReceiver(result);
                    receiveQueue.BeginReceive();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            };
            receiveQueue.BeginReceive();
        }
    }
}