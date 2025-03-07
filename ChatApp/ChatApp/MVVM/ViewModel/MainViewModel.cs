using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        private Server _server;
        public string Username { get; set; }
        public string Message { get; set; }
        public MainViewModel()
        {
            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<string>();
            _server = new Server();
            _server.connectedEvent += Connected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectedEvent += Disconnected;
            ConnectToServerCommand = new RelayCommand(c => _server.ConnectToServer(Username),c=>!string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(c => _server.SendMessage(Message),c=>!string.IsNullOrEmpty(Message));
            
        }
        private void MessageReceived()
        {
            var msg = _server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }
        private void Disconnected()
        {
            var uid = _server.packetReader.ReadMessage();
            var user = Users.Where(c => c.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
        private void Connected()
        {
            var user = new User()
            {
                Username = _server.packetReader.ReadMessage(),
                UID = _server.packetReader.ReadMessage()
            };
            if(!Users.Any(u=>u.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }

        }

    }
}
