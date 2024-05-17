
using Messenger_main.Views;
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
using System.Xml.Linq;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace Messenger_main.ViewModels
{
    internal class AdminPageViewModel : BindingHelper
    {
        #region Interface vars
        List<Socket> clients = new List<Socket>();
        private Socket socket;

        private ObservableCollection<string> Massages = new ObservableCollection<string>();
        public ObservableCollection<string> messages
        {
            get { return Massages; }
            set { Massages = value;
                OnPropertyChanged();
            }
            
        }

        private ObservableCollection<string> Users = new ObservableCollection<string>();

        public ObservableCollection<string> users
        {
            get { return Users; }
            set { Users = value;
                
                foreach (var item in clients)
                {
                    SendMessage(item, "!!!clear!!!");
                    foreach (var iten in users)
                        SendMessage(item, $"!{iten}");
                }
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

        private Visibility Logv = Visibility.Hidden;

        public Visibility logv
        {
            get { return Logv; }
            set { Logv = value;
                OnPropertyChanged();
            }
        }

        private Visibility Mesv = Visibility.Visible;

        public Visibility mesv
        {
            get { return Mesv; }
            set { Mesv  = value;
                OnPropertyChanged();
            }
        }

        #endregion
        public AdminPageViewModel(string name) 
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 6464);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);

            showlogs = new CommandHelper(_ => ChangeMode());
            send = new CommandHelper(_ => Send());
            exit = new CommandHelper(_ => Exit());

            users.Add(name);

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
                if (message.First() == '@')
                {
                    if (message.Contains("@/disconnect"))
                    {
                        logs.Add($"Пользователь {message.Split('|')[1]} отключается");
                        users.Remove(message.Split('|')[1]);
                        SendMessage(client, "@shutup");
                        client.Close();
                    }
                    else
                    {
                        users.Add(message.Substring(1).Split("|").First());
                        logs.Add($"Пользователь {message.Substring(1).Split("|").First()} подключчается");
                    }


                }
                else
                {
                    messages.Add(message);
                    foreach (var item in clients)
                    {
                        SendMessage(item, message);
                    }
                }
                
            }
        }

        void Send()
        {
            if (message == "/disconnect")
                Exit();
            else
            {
                messages.Add($"{DateTime.Now.ToString("g")} | {message}");
                foreach (var item in clients)
                {
                    SendMessage(item, $"{DateTime.Now.ToString("g")} | {message}");
                }

            }
        }

        void Exit()
        {
            var Curpage = new AuthorizationPage();
            (Application.Current.MainWindow as MainWindow).vm.curpage = Curpage;
        }


        async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }
    }
}
