// Client

using System.Net;
using System.Net.Sockets;
using System.Text;

IPAddress address = IPAddress.Loopback; //IPAddress.Parse("192.168.0.158"); //
int port = 5000;

using TcpClient client = new();

Console.Write("Input your nickname: ");
string userNickname = Console.ReadLine();

StreamReader? reader = null;
StreamWriter? writer = null;
NetworkStream? fileStream = null;

try
{
    client.Connect(address, port);
    reader = new StreamReader(client.GetStream());
    writer = new StreamWriter(client.GetStream());
    fileStream = client.GetStream();

    if (reader is null || writer is null) return;

    Task.Run(() => ReseiveMessageAsync(reader));
    await SendMewssageAsync(writer);
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}

writer?.Close();
reader?.Close();



async Task SendMewssageAsync(StreamWriter writer)
{
    await writer.WriteLineAsync(userNickname);
    await writer.FlushAsync();

    SendFileAsync(fileStream, writer, "file.jpg");

    while (true)
    {
        Console.Write("Input message: ");
        string? message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}

void SendFileAsync(NetworkStream writer, StreamWriter swriter, string fileName)
{
    int size = 1024;

    byte[] fileBin = File.ReadAllBytes(fileName);
    byte[] fileSize = BitConverter.GetBytes(fileBin.Length);

    swriter.WriteLineAsync("<file>");
    swriter.FlushAsync();
    writer.Write(fileSize);
    writer.FlushAsync();

    int realSize = 0;
    int restSize = fileBin.Length;

    while (restSize > 0)
    {
        int currSize = Math.Min(size, restSize);
        writer.Write(fileBin, realSize, currSize);

        realSize += currSize;
        restSize -= currSize;
    }

}


async Task ReseiveMessageAsync(StreamReader reader)
{
    while(true)
    {
        try
        {
            string? message = await reader.ReadLineAsync();
            if (String.IsNullOrEmpty(message)) continue;
            PrintMessage(message);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

void PrintMessage(string message)
{
    if(OperatingSystem.IsWindows())
    {
        var position = Console.GetCursorPosition();

        int row = position.Top;
        int column = position.Left;

        Console.MoveBufferArea(0, row, column, 1, 0, row + 1);
        Console.SetCursorPosition(0, row);
        Console.WriteLine(message);
        Console.SetCursorPosition(column, row + 1);
    }
    else
        Console.WriteLine(message);
    
}