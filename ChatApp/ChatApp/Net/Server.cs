using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader packetReader;
        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectedEvent;
        public Server()
        {
            _client = new TcpClient();
        }
        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                packetReader = new(_client.GetStream());
                if (!string.IsNullOrEmpty(username))
                {
                    var connectionPacket = new PacketBuilder();

                    connectionPacket.WriteOpCode(0);
                    connectionPacket.WriteStr(username);
                    _client.Client.Send(connectionPacket.GetPacketBytes());
                }
                
            }
            Task.Run(() => ReadPackets());
        }
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectedEvent.Invoke();
                            break;
                        default:
                            Console.WriteLine("Ermm");
                            break;
                    }
                }
            });
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
