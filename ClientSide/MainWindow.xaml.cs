using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private string username;

        public MainWindow(string username)
        {
            InitializeComponent();
            this.username = username;
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 27001);
                _ = Task.Run(() => ReceiveMessages());
                SendMessage($"{username} has joined the chat.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to the server: {ex.Message}");
            }
        }

        private async void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    var message = await ReceiveStringAsync(client.GetStream());
                    Dispatcher.Invoke(() =>
                    {
                        ChatTextBox.AppendText(message + Environment.NewLine);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection to the server lost: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = MessageTextBox.Text;
                SendMessage($"{username}: {message}");
                MessageTextBox.Clear(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}");
            }
        }

        private async Task SendStringAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task<string> ReceiveStringAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[4096];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        private async void SendMessage(string message)
        {
            await SendStringAsync(message);
        }
    }
}
