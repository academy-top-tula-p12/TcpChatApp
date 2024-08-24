using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TcpChatListenerApp
{
    public class TcpChatServer
    {
        IPAddress address = IPAddress.Loopback;
        int port = 5000;
        TcpListener listener;
        List<TcpChatClient> clients;

        public TcpChatServer()
        {
            listener = new TcpListener(new IPEndPoint(address, port));
            clients = new List<TcpChatClient>();
        }

        public async Task ListenAsync()
        {

        }

        public async Task MessageSendAsync(string id, string message)
        {

        }

        public void ClientClose(string id)
        {

        }

        public void ClientsAllClose()
        {

        }
    }
}
