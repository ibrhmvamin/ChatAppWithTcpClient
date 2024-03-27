using System;
using System.Net.Sockets;
using System.Windows;

namespace ChatClient
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            try
            {
                var username = UsernameTextBox.Text;
                var mainWindow = new MainWindow(username);
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to the server: {ex.Message}");
            }
        }
    }
}
