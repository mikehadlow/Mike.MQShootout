using System.Threading;

namespace Mike.MQShootout
{
    public class ShootoutTest
    {
        private readonly IMessageReceiver<byte[]> messageReceiver;
        private readonly IMessageSender<byte[]> messageSender;

        public ShootoutTest(IMessageReceiver<byte[]> messageReceiver, IMessageSender<byte[]> messageSender)
        {
            this.messageReceiver = messageReceiver;
            this.messageSender = messageSender;
        }

        public void Run(int messageSizeInBytes, long numberOfMessages)
        {
            var receivingMachine = new MessageReceivingMachine(messageReceiver, numberOfMessages);
            var sendingMachine = new MessageSendingMachine(messageSender);

            Thread.Sleep(messageSizeInBytes);

            sendingMachine.RunTest(1000, numberOfMessages);

            receivingMachine.WaitForCompletion();            
        }
    }
}