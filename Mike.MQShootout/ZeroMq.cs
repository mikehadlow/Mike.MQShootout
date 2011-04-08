using System;
using System.Threading;
using ZMQ;

namespace Mike.MQShootout
{
    public class ZeroMq : IMessageReceiver<byte[]>, IMessageSender<byte[]>, IDisposable
    {
        private const string publisherUrl = @"tcp://*:5556";
        private const string subscriberUrl = @"tcp://localhost:5556";
//        private const string publisherUrl = @"inproc://my_publisher";
//        private const string subscriberUrl = @"inproc://my_publisher";

        private readonly Context context;
        private readonly Socket sender;
        private Socket receiver;

        private Thread serverThread;

        public ZeroMq()
        {
            context = new Context(1);

            sender = context.Socket(SocketType.PUB);
            sender.Bind(publisherUrl);
        }

        public void Send(byte[] message)
        {
            sender.Send(message);
        }

        public void ReceiveMessageAlt(Action<byte[]> messageReceiver)
        {
            receiver = context.Socket(SocketType.SUB);
            receiver.Subscribe(new byte[0]);
            receiver.Connect(subscriberUrl);

            receiver.PollInHandler += (socket, revents) =>
            {
                var message = receiver.Recv();
                messageReceiver(message);
            };
        }

        public void ReceiveMessage(Action<byte[]> messageReceiver)
        {
            serverThread = new Thread(() =>
            {
                // 0mq: always start subscribers first.
                receiver = context.Socket(SocketType.SUB);
                receiver.Subscribe(new byte[0]);
                receiver.Connect(subscriberUrl);

                while (true)
                {
                    // going to spend most of its time blocked here
                    var message = receiver.Recv(); 
                    messageReceiver(message);
                }
            });
            serverThread.Start();
        }

        void receiver_PollInHandler(Socket socket, IOMultiPlex revents)
        {
            throw new NotImplementedException();
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (disposed) return;
            if (serverThread.IsAlive)
            {
                serverThread.Abort();
            }
            if(sender != null) sender.Dispose();
            if(receiver != null) receiver.Dispose();
            if(context != null) context.Dispose();
            disposed = true;
        }

        public static string GetZmqVersion()
        {
            return ZHelpers.Version();
        }
    }
}