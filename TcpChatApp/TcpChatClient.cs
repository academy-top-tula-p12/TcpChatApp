using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatListenerApp
{
    public class TcpChatClient
    {
        TcpClient client;
        TcpChatServer server;
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public StreamWriter Writer { get; }
        public StreamReader Reader { get; }

        public TcpChatClient(string name, TcpClient client, TcpChatServer server)
        {
            this.client = client;
            this.server = server;
            Name = name;

            var stream = client.GetStream();
            Writer = new(stream);
            Reader = new(stream);
        }

        public async Task ProcessAsync()
        {

        }

        public void Close()
        {

        }
    }
}
