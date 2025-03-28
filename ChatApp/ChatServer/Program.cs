using ChatServer.Net.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ChatServer
{
    internal class Program
    {
        static TcpListener _listener;
        static List<Client> _clients;
        static void Main(string[] args)
        {
            _clients = new();
            _listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 7891);
            _listener.Start();
            Console.WriteLine("Listening on localhost:7891");
            while (true)
            {
                var _client = new Client(_listener.AcceptTcpClient());
                _clients.Add(_client);
                BroadCastConnected();
            }
            static void BroadCastConnected()
            {
                try
                {
                    foreach (var client in _clients)
                    {
                        foreach (var clientClient in _clients)
                        {
                            var packet = new PacketBuilder();
                            packet.WriteOpCode(1);
                            packet.WriteStr(clientClient.UserName);
                            packet.WriteStr(clientClient.UID.ToString());
                            client.ClientSocket.Client.Send(packet.GetPacketBytes());
                        }
                    }
                }
                catch
                {

                }
                
            }
        }
        public static void BroadCastMessage(string msg)
        {
            foreach (var client in _clients)
            {
                var packet = new PacketBuilder();
                packet.WriteOpCode(5);
                packet.WriteStr(msg);
                client.ClientSocket.Client.Send(packet.GetPacketBytes());
            }
        }
           
        public static void BroadCastDisconnect(string uid)
        {
            try
            {
                var dClient = _clients.Where(c => c.UID.ToString() == uid).FirstOrDefault();
                if (dClient != null)
                {
                    _clients.Remove(dClient);
                    foreach (var client in _clients)
                    {
                        var packet = new PacketBuilder();
                        packet.WriteOpCode(10);
                        packet.WriteStr(uid);
                        client.ClientSocket.Client.Send(packet.GetPacketBytes());

                    }
                    BroadCastMessage($"{dClient.UserName} disconnected!");
                }
            }
            catch
            {

            }
        }
    }
}
