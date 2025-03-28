using ChatClient.Net.IO;
using ChatClientConsole.Net;
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader packetReader;
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public List<string> Messages { get; set; } = new List<string>();
        public Server()
        {
            _client = new TcpClient();
        }
        public bool _connected = false;
        public void DisconnectFromServer()
        {
            if (_client.Connected)
            {
                Messages.Clear();
                Users.Clear();
                _client.Dispose();
                _client = null;
                packetReader.Dispose();
                packetReader = null;
                _connected = false;
                _client = new TcpClient();
                Console.WriteLine("Disconnected");
                Console.Clear();
            }
        }
        public void ConnectToServer(string username,string ip = "127.0.0.1",int port=7891)
        {
            if (!_client.Connected)
            {
                try
                {
                    //_client.Connect("0.tcp.eu.ngrok.io", 14400);
                    _client.Connect(ip, port);
                    packetReader = new(_client.GetStream());
                    if (!string.IsNullOrEmpty(username))
                    {
                        var connectionPacket = new PacketBuilder();

                        connectionPacket.WriteOpCode(0);
                        connectionPacket.WriteStr(username);
                        _client.Client.Send(connectionPacket.GetPacketBytes());
                        _connected = true;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Failed to reach host on port {port}");
                    Console.WriteLine(e);
                    Thread.Sleep(6000);
                    Console.Clear();
                    return;
                }

            }
            //Task.Run(() => ReadPackets());
            Task.Run(() => SendMessages());
            Task.Run(() => RefreshScreen());
            ReadPackets();
        }
        private void RefreshScreen()
        {
            const int messagesToDisplay = 25;
            while (true)
            {
                if (Messages.Count > messagesToDisplay)
                {
                    try
                    {
                        Console.Clear();
                        DisplayUsers();
                        for (int i = 0; i <= Messages.Count - messagesToDisplay; i++)
                        {
                            Messages.RemoveAt(i);
                        }
                        foreach (var msg in Messages)
                        {
                            Console.WriteLine(msg);
                        }
                    }
                    catch
                    {

                    }
                }
                //Thread.Sleep(50);
            }
        }
        private void SendMessages()
        {
            while (true)
            {
                Console.Write("");
                string msg = Console.ReadLine();
                if (msg == "!exit")
                {
                    DisconnectFromServer();
                    break;
                }
                if(string.IsNullOrEmpty(msg))
                {
                    msg = "_";
                }
                SendMessage(msg);
            }
        }
        private void MessageReceived()
        {
            var msg = packetReader.ReadMessage();
            Messages.Add(msg);
            Console.WriteLine(msg);
        }
        private void Disconnected()
        {
            var uid = packetReader.ReadMessage();
            var user = Users.Where(c => c.UID == uid).FirstOrDefault();
            Users.Remove(user);
            Console.Clear();
            DisplayUsers();
        }
        private void DisplayUsers()
        {
            string users = "";
            foreach (var u in Users)
            {
                users += $"{u.Username};";
            }
            Console.WriteLine($"Connected users: {users}");
        }
        private void Connected()
        {
            var user = new User()
            {
                Username = packetReader.ReadMessage(),
                UID = packetReader.ReadMessage()
            };
            if (!Users.Any(u => u.UID == user.UID))
            {
                Users.Add(user);
            }
            Console.Clear();
            DisplayUsers();
        }
        private void ReadPackets()
        {
            while (true)
            {
                try
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            Connected();
                            break;
                        case 5:
                            MessageReceived();
                            break;
                        case 10:
                            Disconnected();
                            break;
                        default:
                            Console.WriteLine("Ermm");
                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }
        public void SendMessage(string msg)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(5);
            packet.WriteStr(msg);
            _client.Client.Send(packet.GetPacketBytes());
        }
    }
}
