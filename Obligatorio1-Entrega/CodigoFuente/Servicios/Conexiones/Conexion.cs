using Servicios.Interfaces;
using Servicios.Protocolo;
using System.Net.Sockets;
using System.Text;

namespace Servicios.Conexiones
{
    public class Conexion: IConexion
    {
        public int Id { get; set; }
        public ISocketHelper SocketHelp { get; set; }
        public Object EntidadConectada { get; set; }
        public Conexion() { }

        public Conexion(Socket socket) : base()
        {
            SocketHelp = FabricaInterfaces.FabricaSocketHelper(socket);
        }
        public Conexion(int id, Socket socket)
        {
            Id = id;
            SocketHelp = FabricaInterfaces.FabricaSocketHelper(socket);
        }

        public Conexion(int id, Socket socket, Object entidadConectada)
        {
            Id = id;
            SocketHelp = FabricaInterfaces.FabricaSocketHelper(socket);
            EntidadConectada = entidadConectada;
        }
        public void EnviarTransmision(Transmision t)
        {
            SocketHelp.Send(t.HeaderByte);
            SocketHelp.Send(t.BodyByte);
        }
        public void EnviarArchivo(string? ruta)
        {
            IGestorEnvioArchivos _gestorEnvioArchivos = FabricaInterfaces.FabricaGestorEnvioArchivo();
            _gestorEnvioArchivos.EnviarArchvivo(ruta, SocketHelp);
        }

        public Transmision EscucharMensajes(int largoHeader)
        {
            byte[] headerByte = SocketHelp.Receive(largoHeader);

            Header header = new Header();
            header.DecodificarData(headerByte);

            int dataSize = header.IDataLength;

            byte[] data = SocketHelp.Receive(dataSize);
            var msg = Encoding.UTF8.GetString(data);
            Transmision t = new Transmision();
            t.HeaderT = header;
            t.BodyT = msg;
            return t;                
        }
    }
}
