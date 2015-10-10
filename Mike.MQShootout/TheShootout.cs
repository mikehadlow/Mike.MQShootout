using System;

namespace Mike.MQShootout
{
    public class TheShootout : IDisposable
    {
        public void Dispose()
        {
            Dispose();
        }

        public void NullMQ(int messageSize, long numberOfMessages)
        {
            var nullMq = new NullMq<byte[]>();

            new ShootoutTest(nullMq, nullMq).Run(messageSize, numberOfMessages);
        }

        public void Msmq(int messageSize, long numberOfMessages)
        {
            var msmq = new Msmq<byte[]>();

            new ShootoutTest(msmq, msmq).Run(messageSize, numberOfMessages);
        }


        public void ZeroMq(int messageSize, long numberOfMessages)
        {
            Console.WriteLine("ZMQ Version: {0}", MQShootout.ZeroMq.GetZmqVersion());

            using(var zeroMq = new ZeroMq())
            {
                new ShootoutTest(zeroMq, zeroMq).Run(messageSize, numberOfMessages);
            }
        }


        public void ActiveMq(int messageSize, long numberOfMessages)
        {
            using (var activeMq = new ActiveMq())
            {
                new ShootoutTest(activeMq, activeMq).Run(messageSize, numberOfMessages);
            }
        }

        public void RabbitMq(int messageSize, long numberOfMessages)
        {
            using (var rabbitMq = new Rabbit())
            {
                new ShootoutTest(rabbitMq, rabbitMq).Run(messageSize, numberOfMessages);
            }
        }
    }
}
