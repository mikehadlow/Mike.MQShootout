using System;
using System.Threading;

namespace Mike.MQShootout
{
    public class TheShootout
    {
        // baseline: 16,474,464 per second
        public void NullMQ()
        {
            const long numberOfMessages = 10000000;

            var nullMq = new NullMq<byte[]>();
            var receivingMachine = new MessageReceivingMachine(nullMq, numberOfMessages);
            var sendingMachine = new MessageSendingMachine(nullMq);

            sendingMachine.RunTest(1000, numberOfMessages);
        }

        // With 1000000 / 100 bytes
        // Sent 14665 per second
        // Received 10641 per second
        //
        // Sent 1000000 messages in 87672 ms / 1000 bytes
        // 11406 per second
        // 1000000 messages received in 142412 ms
        // Received 7021 per second
        // (had to increase the default msmq storage size to 2G)
        public void Msmq()
        {
            const long numberOfMessages = 200000;

            var msmq = new Msmq<byte[]>();
            var receivingMachine = new MessageReceivingMachine(msmq, numberOfMessages);
            var sendingMachine = new MessageSendingMachine(msmq);

            sendingMachine.RunTest(10000, numberOfMessages);

            receivingMachine.WaitForCompletion();
        }

        // Sent 1000000 messages in 4121 ms / 1000 bytes
        // 242659 per second
        // 1000000 messages received in 11274 ms
        // Received 88699 per second
        public void ZeroMq()
        {
            const long numberOfMessages = 1000000;

            Console.WriteLine("ZMQ Version: {0}", MQShootout.ZeroMq.GetZmqVersion());

            using(var zeroMq = new ZeroMq())
            {
                var receivingMachine = new MessageReceivingMachine(zeroMq, numberOfMessages);
                var sendingMachine = new MessageSendingMachine(zeroMq);

                Thread.Sleep(1000);

                sendingMachine.RunTest(1000, numberOfMessages);

                receivingMachine.WaitForCompletion();
            }
        }

        //Sent 1000000 messages in 154979 ms
        //6452 per second
        //1000000 messages received in 154955 ms
        //Received 6453 per second
        public void ActiveMq()
        {
            const long numberOfMessages = 1000000;

            using (var activeMq = new ActiveMq())
            {
                var receivingMachine = new MessageReceivingMachine(activeMq, numberOfMessages);
                var sendingMachine = new MessageSendingMachine(activeMq);

                Thread.Sleep(1000);

                sendingMachine.RunTest(1000, numberOfMessages);

                receivingMachine.WaitForCompletion();
            }
        }
    }
}