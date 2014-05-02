using System;
using System.Net.Sockets;

namespace SocketService {
    public interface ISocketService {
        void SendMessage(Socket targetSocket, string messageContent, Action<string> messageSentCallBack);
        void ReceiveMessage(Socket sourceSocket, Action<string> messageReceivedCallback);
    }
}