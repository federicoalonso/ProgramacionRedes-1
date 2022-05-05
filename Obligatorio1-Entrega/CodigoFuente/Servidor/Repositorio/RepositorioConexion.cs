using Servicios.Interfaces;
using Servidor.Dominio;

namespace Servidor.Repositorio
{
    internal class RepositorioConexion
    {
        private int _idCliente = 0;
        private List<IConexion> _conexiones = new List<IConexion>();
        private static RepositorioConexion Instancia = null;
        private static readonly object CandadoInstancia = new Object();
        private static readonly object Candadoconexiones = new Object();

        private RepositorioConexion()
        {
            _conexiones = new List<IConexion>();
        }
        internal static RepositorioConexion ObtenerInstancia()
        {
            if (RepositorioConexion.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (RepositorioConexion.Instancia == null)
                    {
                        RepositorioConexion.Instancia = new RepositorioConexion();
                    }
                }
            }
            return RepositorioConexion.Instancia;
        }

        internal IConexion AltaConexion(IConexion conexion)
        {
            lock (Candadoconexiones)
            {
                Interlocked.Increment(ref _idCliente);
                conexion.Id = _idCliente;
                _conexiones.Add(conexion);
            }
            return conexion;
        }

        internal void BajaConexion(IConexion conexion)
        {
            lock (Candadoconexiones)
            {
                _conexiones.Remove(conexion);
                conexion.SocketHelp.Close();
            }
        }

        internal void CerrarConexiones()
        {
            lock (Candadoconexiones)
            {
                for (int i= _conexiones.Count; i> 0; i--){
                    BajaConexion(_conexiones[i-1]);
                }
            }
        }

        internal IConexion BuscarPorUsuario(Usuario usuario)
        {
            foreach(IConexion conn in _conexiones)
            {
                if(conn.EntidadConectada != null)
                {
                    Usuario usuarioConectado = (Usuario)conn.EntidadConectada;
                    if(usuario.Id == usuarioConectado.Id)
                        return conn;
                }
            }
            return null;
        }

        internal void Logout(int usuarioId)
        {
            foreach (IConexion conn in _conexiones)
            {
                if (conn.EntidadConectada != null)
                {
                    Usuario usuarioConectado = (Usuario)conn.EntidadConectada;
                    if (usuarioId == usuarioConectado.Id)
                        conn.EntidadConectada = null;
                }
            }
        }

        internal void DesconectarUsuario(int id)
        {
            foreach (IConexion conn in _conexiones)
            {
                if (conn.EntidadConectada != null)
                {
                    Usuario usuarioConectado = (Usuario)conn.EntidadConectada;
                    if (id == usuarioConectado.Id)
                        BajaConexion(conn);
                }
            }
        }

        internal IConexion ObtenerConexionUsuario(int id)
        {
            foreach (IConexion conn in _conexiones)
            {
                if (conn.EntidadConectada != null)
                {
                    Usuario usuarioConectado = (Usuario)conn.EntidadConectada;
                    if (id == usuarioConectado.Id)
                        return conn;
                }
            }
            return null;
        }
    }
}
