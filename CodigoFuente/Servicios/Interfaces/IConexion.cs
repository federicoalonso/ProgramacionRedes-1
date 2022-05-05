using Servicios.Conexiones;
using Servicios.Protocolo;

namespace Servicios.Interfaces
{
    public interface IConexion
    {
        int Id { get; set; }
        Object EntidadConectada { get; set; }
        ISocketHelper SocketHelp { get; set; }

        void EnviarTransmision(Transmision t);
        Transmision EscucharMensajes(int largoHeader);
        void EnviarArchivo(string? ruta);
    }
}
