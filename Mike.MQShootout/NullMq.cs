using System;

namespace Mike.MQShootout
{
    // A fake MQ that does nothing but pass the message straight through.
    public class NullMq<T> : IMessageSender<T>, IMessageReceiver<T>
    {
        private Action<T> messageReceiver;

        public void Send(T message)
        {
            messageReceiver(message);
        }

        public void ReceiveMessage(Action<T> messageReceiver)
        {
            this.messageReceiver = messageReceiver;
        }
    }
}