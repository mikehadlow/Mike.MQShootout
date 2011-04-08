namespace Mike.MQShootout
{
    public interface IMessageSender<in T>
    {
        // send should be an async operation
        void Send(T message);
    }
}