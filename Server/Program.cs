using System;
using SocketService;

namespace Server {
    class Program {
        private static void Main(string[] args) {
            new ServerSocket().Bind("127.0.0.1", 11000).Listen(1000).Start(
                message => {
                    Console.WriteLine("received: {0}", message);
                    return string.Format("{0} reply", message);
                },
                reply => Console.WriteLine("sent: {0}", reply));
        }
    }
}
