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

        public CommandHelper newbut { get; set; }
        public CommandHelper conbut { get; set; }


        public AuthorizationPageViewModel()
        {
            newbut = new CommandHelper(_ => newChat());
            conbut = new CommandHelper(_ => connectTo());
        }

        void newChat()
        {
            if (name == null)
            {
                MessageBox.Show("Введите значение в поле имени");
                return;
            }
            var Curpage = new AdminPage(name);
            (Application.Current.MainWindow as MainWindow).vm.curpage = Curpage;
        }

        void connectTo()
        {
            if (ip == null)
            {
                MessageBox.Show("Введите значение в поле IP адреса");
                return;
            }
            if (name == null)
            {
                MessageBox.Show("Введите значение в поле имени");
                return;
            }
            try
            {
                var Curpage = new UserPage(ip, name);
                (Application.Current.MainWindow as MainWindow).vm.curpage = Curpage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Некоррекный IP! " + ex.Message);
            }
        }
    }
}
