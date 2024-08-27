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
        public string Name { get; set; } = null!;
        public StreamWriter Writer { get; }
        public StreamReader Reader { get; }

        async Task SendAndWrite(string message)
        {
            Console.WriteLine(message);
            await server.MessageSendAsync(Id, message);
        }

        public TcpChatClient(TcpClient client, TcpChatServer server)
        {
            this.client = client;
            this.server = server;

            var stream = client.GetStream();
            Writer = new(stream);
            Reader = new(stream);
        }

        public async Task ProcessAsync()
        {
            string? message;
            try
            {
                string? userNickname = await Reader.ReadLineAsync();
                if (userNickname is not null)
                {
                    Name = userNickname;
                    message = $"User {Name} in to chat";
                    await SendAndWrite(message);
                }

                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message is null) continue;

                        if(message == "<file>")
                            ReadFile();

                        message = $"{Name}: {message}";
                        await SendAndWrite(message);
                    }
                    catch (Exception ex)
                    {
                        message = $"User {Name} out from chat";
                        await SendAndWrite(message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.ClientClose(Id);
            }
        }

        void ReadFile()
        {
            Console.WriteLine("Server reading file...");

            NetworkStream writer = client.GetStream();
            byte[] fileSizeBin = new byte[4];
            writer.Read(fileSizeBin);
            int size = BitConverter.ToInt32(fileSizeBin, 0);

            byte[] fileBin = new byte[size];

            int restSize = size;
            int bufferSize = 1024;
            int realSize = 0;

            int readSize = 0;

            while(restSize > 0)
            {
                int currSize = Math.Min(bufferSize, restSize);

                //if(currSize > client.Available)
                //    currSize = client.Available;

                writer.Read(fileBin, realSize, currSize);
                realSize += currSize;
                restSize -= currSize;
            }
            File.WriteAllBytes("readfile.jpg", fileBin);
            Console.WriteLine("File saved to server");
        }

        public void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
