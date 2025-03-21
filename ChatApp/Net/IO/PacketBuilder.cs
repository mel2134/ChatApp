using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    class PacketBuilder
    {
        MemoryStream _stream;
        public PacketBuilder()
        {
            _stream = new();
        }
        public void WriteOpCode(byte opcode)
        {
            _stream.WriteByte(opcode);
        }
        public void WriteStr(string msg)
        {
            _stream.Write(BitConverter.GetBytes(msg.Length));
            _stream.Write(Encoding.ASCII.GetBytes(msg));
        }
        public byte[] GetPacketBytes()
        {
            return _stream.ToArray();
        }
    }
}
