using Servicios.Interfaces;
using Servicios.Conexiones;
using System.Net.Sockets;
using Servicios.Protocolo;

namespace Servicios
{
    public static class FabricaInterfaces
    {
        public static IConexion FabricaConexion()
        {
            IConexion conexion = new Conexion();
            return conexion;
        }

        public static ISocketHelper FabricaSocketHelper(Socket socket)
        {
            ISocketHelper sh = new SocketHelper(socket);
            return sh;
        }

        public static IGestorConfiguracion FabricaGestorConfiguracion()
        {
            IGestorConfiguracion conf = new GestorConfiguracion();
            return conf;
        }

        public static IGestorEnvioArchivos FabricaGestorEnvioArchivo()
        {
            IGestorEnvioArchivos gest = new GestorEnvioArchivos();
            return gest;
        }
    }
}
