using Messenger_main.Models;
using PRACT_LAB_5.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messenger_main.ViewModels
{
    internal class AdminPageViewModel : BindingHelper
    {

        List<Socket> clients = new List<Socket>();
        private Socket socket;

        private ObservableCollection<UserModel> Users;

        public ObservableCollection<UserModel> users
        {
            get { return Users; }
            set { Users = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> Massages = new ObservableCollection<string>();
        public ObservableCollection<string> messages
        {
            get { return Massages; }
            set { Massages = value;
                OnPropertyChanged();
            }
            
        }

        private ObservableCollection<string> Logs = new ObservableCollection<string>();
        public ObservableCollection<string> logs
        {
            get { return Logs; }
            set
            {
                Logs = value;
                OnPropertyChanged();
            }

        }

        private string Message;

        public string message
        {
            get { return Message; }
            set { Message = value;
                OnPropertyChanged();
            }
        }


        public CommandHelper showlogs { get; set; }
        public CommandHelper send { get; set; }
        public CommandHelper exit { get; set; }

        public Visibility logv { get; set; } = Visibility.Hidden;
        public Visibility mesv { get; set; } = Visibility.Visible;


        public AdminPageViewModel(string name) 
        {
            users = new ObservableCollection<UserModel>();
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 6464);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);

            showlogs = new CommandHelper(_ => ChangeMode());
            send = new CommandHelper(_ => Send());

            UserModel admin = new UserModel(name, socket);
            users.Add(admin);
            socket.Listen(100);
            ListenToClient();
        }

        void ChangeMode()
        {
            if (logv == Visibility.Visible)
            {
                mesv = Visibility.Visible;
                logv = Visibility.Hidden;
            }
            else if (mesv == Visibility.Visible)
            {
                mesv = Visibility.Hidden;
                logv = Visibility.Visible;
            }
        }


        void Exit()
        {
            Application.Current.Shutdown();
        }

        async Task ListenToClient()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                clients.Add(client);
                RecieveMessage(client);
            }
        }

        async Task RecieveMessage(Socket client)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                if (message[8] == '@')
                {
                    logs.Add(message);
                    continue;
                }
                messages.Add(message);
                foreach (var item in clients)
                {
                    SendMessage(item, message);
                }
            }
        }

        void Send()
        {
            SendMessage(socket, message);
        }


        async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }
    }
}
