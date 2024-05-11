using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_main.Models
{
    internal class UserModel
    {
        public string name { get; set; }
        Socket socket;

        public UserModel(string name, Socket socket)
        {
            this.name = name;
            this.socket = socket;
        }   
    }
}
