using System;
using System.Diagnostics;

namespace Mike.MQShootout
{
    public class MessageSendingMachine
    {
        private readonly IMessageSender<byte[]> messageSender;
        private const long reportingInterval = 1000000;

        public MessageSendingMachine(IMessageSender<byte[]> messageSender)
        {
            this.messageSender = messageSender;
        }

        public void RunTest(int messageSize, long numberToSend)
        {
            var message = new byte[messageSize];

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (long i = 0; i < numberToSend; i++)
            {
                if (i%reportingInterval == 0)
                {
                    Console.WriteLine("Sent {0}", i);
                }

                messageSender.Send(message);
            }

            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Sent {0} messages in {1} ms", numberToSend, elapsedMilliseconds);
            Console.WriteLine("{0} per second", elapsedMilliseconds == 0 ? 0L : (1000 * numberToSend) / elapsedMilliseconds);
        }
    }
}