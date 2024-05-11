using Messenger_main.Models;
using PRACT_LAB_5.ViewModels.Helpers;
using System;
using System.Collections.Generic;
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
        List<UserModel> users { get; set; }
        List<Socket> clients = new List<Socket>();
        private Socket socket;

        public AdminPageViewModel() 
        {
            users = new List<UserModel>();
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 6464);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(100);
            ListenToClient();
        }

        async Task ListenToClient()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                clients.Add(client);
            }
        }

        async Task RecieveMessage(Socket client)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                /*имялистбокса.Items.Add($"[Сообщение от {client.RemoteEndPoint}]: {message}");*/

                foreach (var item in clients)
                {
                    SendMessage(item, message);
                }
            }
        }


        async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }
    }
}
