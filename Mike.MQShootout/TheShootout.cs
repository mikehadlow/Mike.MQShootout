using System;

namespace Mike.MQShootout
{
    public class TheShootout
    {
        // baseline: 16,474,464 per second
        public void NullMQ()
        {
            const long numberOfMessages = 10000000;

            var nullMq = new NullMq<byte[]>();

            new ShootoutTest(nullMq, nullMq).Run(1000, numberOfMessages);
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
            const long numberOfMessages = 1000000;

            var msmq = new Msmq<byte[]>();

            new ShootoutTest(msmq, msmq).Run(1000, numberOfMessages);
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
                new ShootoutTest(zeroMq, zeroMq).Run(1000, numberOfMessages);
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
                new ShootoutTest(activeMq, activeMq).Run(1000, numberOfMessages);
            }
        }

        public void RabbitMq()
        {
            const long numberOfMessages = 1000000;

            using (var rabbitMq = new Rabbit())
            {
                new ShootoutTest(rabbitMq, rabbitMq).Run(1000, numberOfMessages);
            }
        }
    }
}