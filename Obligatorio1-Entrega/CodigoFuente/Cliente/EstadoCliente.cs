using Servicios.DTOs;

namespace Cliente
{
    internal class EstadoCliente
    {
        internal UsuarioDTO _usuario { get; set; }
        internal ClienteServidor _conexion { get; set; }
        private static readonly object CandadoInstancia = new Object();
        internal object ConsolaCandado = new Object();
        private static EstadoCliente Instancia = null;

        private EstadoCliente()
        { 
            _conexion = new ClienteServidor();
        }
        internal static EstadoCliente ObtenerInstancia()
        {
            if (EstadoCliente.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (EstadoCliente.Instancia == null)
                    {
                        EstadoCliente.Instancia = new EstadoCliente();
                    }
                }
            }
            return EstadoCliente.Instancia;
        }
    }
}
