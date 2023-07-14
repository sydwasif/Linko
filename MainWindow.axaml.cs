using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;
using Avalonia.Controls;
using System.Linq;

namespace Linko
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // TCP server port
            int port = 1111;


            status.Text = "TCP server port:" + port.ToString();



            // Create a new TCP chat server

            var server = new ChatServer(IPAddress.Parse("0.0.0.0"), port);
            status.Text = status.Text + "\n" + server.Address.ToString();

            // Start the server
            status.Text = status.Text + "\nServer starting...";
            server.Start();
            status.Text = status.Text + "\nDone!";


            Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            /*for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    server.Restart();
                    Console.WriteLine("Done!");
                    continue;
                }

                // Multicast admin message to all sessions
                line = "(admin) " + line;
                server.Multicast(line);
            }*/

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
        }



        public void checkit(object sender, EventArgs e)
        {
            // check.IsVisible = false; 
        }

    }
    class ChatSession : TcpSession
    {
        
        public ChatSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {

            // Send invite message
            string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            Console.Write(message);
            SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // Multicast message to all connected sessions
            Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Disconnect();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }

    class ChatServer : TcpServer
    {
        public ChatServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new ChatSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }

   
}