using System;

namespace Mike.MQShootout
{
    public interface IMessageReceiver<T>
    {
        void ReceiveMessage(Action<T> messageReceiver);
    }
}