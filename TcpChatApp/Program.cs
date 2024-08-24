// Listener
using System.Net;
using System.Net.Sockets;
using TcpChatListenerApp;

TcpChatServer server = new();
await server.ListenAsync();