using Servicios.Interfaces;
using System.Net.Sockets;

namespace Servicios.Conexiones
{
    public class SocketHelper : ISocketHelper
    {
        private readonly Socket _socket;

        public SocketHelper(Socket socket)
        {
            _socket = socket;   
        }

        public void Send(byte[] data)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                var sent = _socket.Send(
                    data,
                    offset,
                    data.Length - offset,
                    SocketFlags.None);
                if (sent == 0)
                    throw new Exception("Conexion perdida");
                offset += sent;
            }
        }

        public byte[] Receive(int length)
        {
            int offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                var received = _socket.Receive(
                    data,
                    offset,
                    length - offset,
                    SocketFlags.None);
                if (received == 0)
                {
                    throw new Exception("Conexion perdida");
                }
                offset += received;
            }
            return data;
        }

        public void Close()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
