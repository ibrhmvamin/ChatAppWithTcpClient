using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        private static readonly Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();

        static async Task Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, 27001);
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            string userName = null;
            try
            {
                userName = await ReceiveStringAsync(client.GetStream());
                clients.Add(userName, client);
                Console.WriteLine($"{userName} joined the chat.");

                while (true)
                {
                    var message = await ReceiveStringAsync(client.GetStream());
                    BroadcastMessage($"{userName}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"{userName} left the chat.");
                clients.Remove(userName);
                client.Close();
            }
        }

        static void BroadcastMessage(string message)
        {
            foreach (var client in clients.Values)
            {
                var writer = new BinaryWriter(client.GetStream());
                writer.Write(message);
            }
        }

        static async Task<string> ReceiveStringAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[4096];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
    }
}
