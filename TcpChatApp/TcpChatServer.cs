using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Linq.Expressions;

namespace TcpChatListenerApp
{
    public class TcpChatServer
    {
        IPAddress address = IPAddress.Loopback; //IPAddress.Parse("192.168.0.158"); //
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
            try
            {
                listener.Start();
                Console.WriteLine("Server start...");

                while(true)
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    TcpChatClient client = new(tcpClient, this);

                    clients.Add(client);
                    Task.Run(client.ProcessAsync);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ClientsAllClose();
            }

        }

        public async Task MessageSendAsync(string id, string message)
        {
            foreach(var client in clients)
            {
                if(client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }

        public void ClientClose(string id)
        {
            var client = clients.FirstOrDefault(c => c.Id == id);
            if(client is not null) 
                clients.Remove(client);
            
            client?.Close();
        }

        public void ClientsAllClose()
        {
            foreach(var client in clients)
                client.Close();
            listener.Stop();
        }
    }
}
