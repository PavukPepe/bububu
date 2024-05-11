using PRACT_LAB_5.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Messenger_main.ViewModels
{
    internal class UserPageViewModel : BindingHelper
    {
        private string Messege;

        public string message
        {
            get { return Messege; }
            set { Messege = value; }
        }

        Socket socket;
        public CommandHelper send { get; set; }
        public CommandHelper exit { get; set; }

        private ObservableCollection<string> Messeges = new ObservableCollection<string>();

        public ObservableCollection<string> messages
        {
            get { return Messeges; }
            set { Messeges = value;
                OnPropertyChanged();
            }
        }

        public UserPageViewModel(string ip, string name)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            send = new CommandHelper(_ => Send());
            exit = new CommandHelper(_ => Exit());
            socket.Connect(ip, 6464);
            SendMessage($"@{name}@{ip}");
            RecieveMessage();
        }

        private async Task SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes($"{DateTime.Now.ToString("t")} | {message}");
            await socket.SendAsync(bytes, SocketFlags.None);
        }

        private async void RecieveMessage()
        {
            while(true)
            {
                try
                {
                    byte[] bytes = new byte[65535];
                    await socket.ReceiveAsync(bytes, SocketFlags.None);
                    string mes = Encoding.UTF8.GetString(bytes);
                    messages.Add(mes);

                }
                catch (SocketException e)
                {
                    MessageBox.Show(e.Message);
                    Application.Current.Shutdown();
                    break;
                }

            }
        }

        void Send()
        {
            if (message == "\\disconnect")
            {
                Application.Current?.Shutdown();
            }
            SendMessage(message);
        }
        
        void Exit()
        {
            Application.Current?.Shutdown();    
        }
        
    }
}
