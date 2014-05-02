using System;
using System.Threading;
using SocketService;

namespace Client1 {
    class Program {
        private static ManualResetEvent _signal = new ManualResetEvent(false);
        static void Main(string[] args) {
            int currentReceived = 0;
            IClientSocket client = new ClientSocket().Connect("127.0.0.1", 11000).Start(
                reply => {
                    currentReceived++;
                    Console.WriteLine("received: {0}", reply);
                    if(currentReceived == 10) {
                        _signal.Set();
                    }
                });

            for(int i = 0; i < 10; i++) {
                client.SendMessage("Hello1", message => Console.WriteLine("sent: {0}", message));
            }

            _signal.WaitOne();

            Console.ReadLine();

            client.Shutdown();
        }
    }
}
