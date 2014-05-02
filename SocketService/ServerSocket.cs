using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketService {
    public class ServerSocket: IServerSocket {
        private Socket _innerSocket;
        private Func<string, string> _messageReceivedCallback;
        private ManualResetEvent _newClientSocketSignal;
        private ISocketService _socketService;

        public ServerSocket() {
            _innerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _newClientSocketSignal = new ManualResetEvent(false);
            _socketService = new DefaultSocketService();
        }

        public IServerSocket Listen(int backlog) {
            _innerSocket.Listen(backlog);
            return this;
        }

        public IServerSocket Bind(string address, int port) {
            _innerSocket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            return this;
        }

        public void Start(Func<string, string> messageReceivedCallback, Action<string> replyMessageSentCallback) {
            _messageReceivedCallback = messageReceivedCallback;

            Console.WriteLine("socket is listening address: {0}", _innerSocket.LocalEndPoint);

            while(true) {
                _newClientSocketSignal.Reset();
                try {
                    _innerSocket.BeginAccept(asyncResult => {
                        Socket clientSocket = _innerSocket.EndAccept(asyncResult);
                        Console.WriteLine("----accepted new client.");
                        _newClientSocketSignal.Set();
                        _socketService.ReceiveMessage(clientSocket, result => {
                            string replyMessage = _messageReceivedCallback(result);
                            _socketService.SendMessage(clientSocket, replyMessage, replyMessageSentCallback);
                        });
                    }, _innerSocket);
                }
                catch (SocketException socketException) {
                    Console.WriteLine("Socket send exception, ErrorCode: {0}", socketException.ErrorCode);
                }
                catch (Exception ex) {
                    Console.WriteLine("Unknown socket send exception: {0}", ex);
                }

                _newClientSocketSignal.WaitOne();
            }
        }
    }
}