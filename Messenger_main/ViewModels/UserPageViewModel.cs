using PRACT_LAB_5.ViewModels.Helpers;
using System;
using System.Collections.Generic;
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

        public List<string> messages { get; set; } = new List<string>();
        public UserPageViewModel(string ip)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            send = new CommandHelper(_ => Send());
            exit = new CommandHelper(_ => Exit());
            socket.Connect(ip, 6464);
            RecieveMessage();
        }

        private async Task SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(bytes, SocketFlags.None);
        }

        private async void RecieveMessage()
        {
            while(true)
            {
                byte[] bytes = new byte[65535];
                await socket.ReceiveAsync(bytes, SocketFlags.None);
                string mes = Encoding.UTF8.GetString(bytes);

                messages.Add(mes);
            }
        }

        void Send()
        {
            SendMessage(message);
        }
        
        void Exit()
        {

        }
        
    }
}
