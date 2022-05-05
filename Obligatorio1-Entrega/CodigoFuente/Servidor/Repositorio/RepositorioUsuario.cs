using Servidor.Dominio;

namespace Servidor.Repositorio
{
    internal class RepositorioUsuario
    {
        private List<Usuario> Usuarios { get; set; }
        private static int autonumerado = 0;
        private static RepositorioUsuario Instancia = null;
        private static readonly object CandadoInstancia = new Object();
        private static readonly object CandadoLista = new Object();

        private RepositorioUsuario()
        {
            Usuarios = new List<Usuario>();
        }

        internal static RepositorioUsuario ObtenerInstancia()
        {
            if (RepositorioUsuario.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (RepositorioUsuario.Instancia == null)
                    {
                        RepositorioUsuario.Instancia = new RepositorioUsuario();
                    }
                }
            }
            return RepositorioUsuario.Instancia;
        }

        internal List<Usuario> VerUsuariosOrdenadosPorSeguidores()
        {
            List<Usuario> usuarios = new List<Usuario>();
            lock (CandadoLista)
            {
                usuarios = Usuarios.OrderByDescending(s => s.Seguidores.Count).ToList();
            }

            return usuarios;
        }
        internal Usuario Alta(Usuario usuario)
        {
            lock (CandadoLista)
            {
                Interlocked.Increment(ref autonumerado);
                usuario.Id = autonumerado;
                Usuarios.Add(usuario);
            }
            return usuario;
        }
        internal Usuario VerUsuario(int id)
        {
            return Usuarios.Find(o => o.Id == id);
        }
        internal Usuario BuscarPorNombreUsuario(string NombreUsuario)
        {
            return Usuarios.Find(o => o.NombreUsuario == NombreUsuario);
        }
        internal Usuario BuscarPorNombreUsuarioYContrasenia(Usuario usuario)
        {
            return Usuarios.Find(o => o.NombreUsuario == usuario.NombreUsuario
                                  && o.Contrasenia == usuario.Contrasenia);
        }

        internal Usuario BuscarPorId(int id)
        {
            return Usuarios.Find(o => o.Id == id);
        }

        internal Usuario Modificar(Usuario buscado)
        {
            Usuario aModificar = BuscarPorId(buscado.Id);
            aModificar.NombreUsuario = buscado.NombreUsuario;
            aModificar.NombreReal = buscado.NombreReal;
            aModificar.Bloqueado = buscado.Bloqueado;
            aModificar.Seguidos = buscado.Seguidos;
            aModificar.Seguidores = buscado.Seguidores;
            return aModificar;
        }

        internal object BuscarUsuarioPorCondicion(Usuario usuario, bool incluyente)
        {
            List<Usuario> usuarioList = new List<Usuario>();
            if (incluyente)
            {
                foreach (Usuario usu in Usuarios.Where(o => o.NombreUsuario.ToLower().Contains(usuario.NombreUsuario.ToLower()) &&
                                o.NombreReal.ToLower().Contains(usuario.NombreReal.ToLower())))
                {
                    usuarioList.Add(usu);
                }
            }
            else
            {
                foreach (Usuario usu in Usuarios.Where(o => !o.NombreUsuario.ToLower().Contains(usuario.NombreUsuario.ToLower()) &&
                                !o.NombreReal.ToLower().Contains(usuario.NombreReal.ToLower())))
                {
                    usuarioList.Add(usu);
                }
            }
            return usuarioList;
        }

        internal List<Usuario> ObtenerUsuariosMasActivos(List<Chip> chips, int cantUsuarios)
        {
            List<Usuario> retorno = new List<Usuario> ();
            int[] actividad = new int[Usuarios.Count];
            for(int indice = 0; indice < Usuarios.Count; indice++)
            {
                foreach (Chip chip in chips)
                {
                    if (Usuarios[indice].Id == chip.UsuarioId)
                    {
                        actividad[indice]++;
                    }
                }
            }

            int max;
            int posicion = -1;
            int posicionANoConsiderarMas;
            for (int contador = 0; contador < cantUsuarios && contador < Usuarios.Count ; contador++){
                max = 0;
                posicionANoConsiderarMas = -1;
                for (int indice = 0; indice < actividad.Length; indice++) { 
                    if (actividad[indice] > max)
                    {
                        max = actividad[indice];
                        posicion = indice;
                    }
                }
                if (posicion > -1)
                {
                    retorno.Add(Usuarios.ElementAt(posicion));
                    actividad[posicion] = 0;
                }
                posicion = -1;
            }
            return retorno;
        }
    }
}
