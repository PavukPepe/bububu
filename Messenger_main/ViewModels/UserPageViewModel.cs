using Messenger_main.Views;
using PRACT_LAB_5.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
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
        private CancellationTokenSource stoptalking;

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

        private ObservableCollection<string> Users = new ObservableCollection<string>();

        public ObservableCollection<string> users
        {
            get { return Users; }
            set
            {
                Users = value;
                OnPropertyChanged();
            }
        }

        private string name;

        public UserPageViewModel(string ip, string name)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            stoptalking = new CancellationTokenSource();

            send = new CommandHelper(_ => Send());
            exit = new CommandHelper(_ => Exit());
            socket.Connect(ip, 6464);
            this.name = name;

            SendMessage($"@{name}|{ip}");
            RecieveMessage(stoptalking.Token);
        }

        private async Task SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(bytes, SocketFlags.None);
        }

        private async void RecieveMessage(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                try
                {
                    byte[] bytes = new byte[65535];
                    await socket.ReceiveAsync(bytes, SocketFlags.None);
                    string mes = Encoding.UTF8.GetString(bytes);
                    if (mes.Contains("@shutup"))
                    {
                        stoptalking.Cancel();
                    }
                    if (mes == "!!!clear!!!")
                    {
                        users = new ObservableCollection<string>();
                    }
                    else if (mes.First() == '!')
                    {
                        users.Add(mes.Remove('!'));
                    }
                    else 
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

        void Exit()
        {
            SendMessage($"@/disconnect|{name}|");
            var Curpage = new AuthorizationPage();
            (Application.Current.MainWindow as MainWindow).vm.curpage = Curpage;
        }

        void Send()
        {
            if (message == "/disconnect")
                Exit();
            else
                SendMessage($"{DateTime.Now.ToString("g")} | {message}");
        }
        
    }
}
