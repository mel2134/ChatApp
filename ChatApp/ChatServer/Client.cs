using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string UserName { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        PacketReader _reader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _reader = new PacketReader(ClientSocket.GetStream());

            var opcode = _reader.ReadByte();
            UserName = _reader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: {UserName} connected!");
            Task.Run(() => Read());
        }
        void Read()
        {
            while (true)
            {
                try
                {
                    if(_reader != null)
                    {
                        var opcode = _reader.ReadByte();
                        switch (opcode)
                        {
                            case 5:
                                var msg = _reader.ReadMessage();
                                Console.WriteLine($"[{DateTime.Now}]: Received {msg}");
                                Program.BroadCastMessage($"[{DateTime.Now}] [{UserName}]: {msg}");
                                break;
                            default:
                                Console.WriteLine("ERMM");
                                break;
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"[{DateTime.Now}]: {UserName} disconnected!");
                    Program.BroadCastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    _reader = null;

                }
            }
        }
    }
}
