using Messenger_main.Views;
using PRACT_LAB_5.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Messenger_main.ViewModels
{
    internal class AuthorizationPageViewModel : BindingHelper
    {
        public string name { get; set; }

        private string IP;

        public string ip
        {
            get { return IP; }
            set { IP = value; }
        }

        public CommandHelper newbut;
        public CommandHelper conbut;


        public AuthorizationPageViewModel()
        {
            newbut = new CommandHelper(_ => newChat());
            conbut = new CommandHelper(_ => connectTo());
        }

        void newChat()
        {
            var Curpage = new UserPage(ip);
            (Application.Current.MainWindow as MainWindow).vm.curpage = Curpage;
        }

        void connectTo()
        {
            var Curpage = new AdminPage(ip);
            (Application.Current.MainWindow as MainWindow).vm.curpage = Curpage;
        }
    }
}
