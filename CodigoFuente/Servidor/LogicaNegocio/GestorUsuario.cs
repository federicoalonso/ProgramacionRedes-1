using Servicios;
using Servicios.Protocolo;
using Servidor.Dominio;
using Servidor.Repositorio;

namespace Servidor.LogicaNegocio
{
    internal class GestorUsuario
    {
        private RepositorioUsuario _repositorioUsuario = RepositorioUsuario.ObtenerInstancia();
        private const int _largoMin = 5;
        private const int _largoMax = 15;
        private const int _cantUsuarios = 5;

        internal ResultadoOperacion AltaUsuario(Usuario usuario)
        {
            ResultadoOperacion retorno = Validaciones.ValidarLargoTexto(usuario.NombreUsuario, _largoMax, _largoMin, "Nombre de Usuario");
            if (retorno.Codigo < CodigosConstantes.Error)
                retorno = Validaciones.ValidarPassword(usuario.Contrasenia);

            if (retorno.Codigo >= CodigosConstantes.Error)
            {
                retorno.Entidad = null;
                return retorno;
            }
            Usuario buscado = _repositorioUsuario.BuscarPorNombreUsuario(usuario.NombreUsuario);
            if (buscado != null)
            {
                retorno.Codigo = CodigosConstantes.ArgumentoNoValido;
                retorno.Mensaje = "El nombre de usuario ya existe.";
                retorno.Entidad = null;
                return retorno;
            }
            usuario.NombreDeFoto = "";
            retorno.Entidad = _repositorioUsuario.Alta(usuario);
            retorno.Mensaje = "Usuario Guardado con Exito.";
            return retorno;
        }

        internal ResultadoOperacion VerUsuarios()
        {
            ResultadoOperacion retorno = new ResultadoOperacion()
            {
                Codigo = CodigosConstantes.TransmisionOK,
                Mensaje = "",
                Entidad = _repositorioUsuario.VerUsuariosOrdenadosPorSeguidores()
            };

            return retorno;
        }
        internal ResultadoOperacion Login(Usuario usuario)
        {
            ResultadoOperacion retorno = new ResultadoOperacion();
            retorno.Entidad = null;
            Usuario buscado = _repositorioUsuario.BuscarPorNombreUsuarioYContrasenia(usuario);
            GestorConexiones gestorConexiones = new GestorConexiones();
            if (gestorConexiones.VerificarUsuarioConectado(buscado))
            {
                retorno.Codigo = CodigosConstantes.NoAutorizado;
                retorno.Mensaje = "El usuario ya se encuentra conectado.";
            }
            else if (buscado == null)
            {
                retorno.Codigo = CodigosConstantes.ElementoNoEncontrado;
                retorno.Mensaje = "El usuario o contrasenia no coinciden.";
            }
            else if (buscado.Bloqueado)
            {
                retorno.Codigo = CodigosConstantes.Prohibido;
                retorno.Mensaje = "El usuario se encuentra bloqueado.";
            }
            else
            {
                retorno.Codigo = CodigosConstantes.TransmisionOK;
                retorno.Mensaje = "Login efectuado con exito.";
                retorno.Entidad = buscado;
            }
            return retorno;
        }

        internal ResultadoOperacion BloquearUsuario(int id)
        {
            ResultadoOperacion retorno = CambiarEstadoUsuario(true, id);
            return retorno;
        }

        internal ResultadoOperacion AutorizarUsuario(int id)
        {
            ResultadoOperacion retorno = CambiarEstadoUsuario(false, id);
            return retorno;
        }
        private ResultadoOperacion CambiarEstadoUsuario(bool estado, int id)
        {
            ResultadoOperacion retorno = new ResultadoOperacion();
            Usuario buscado = _repositorioUsuario.BuscarPorId(id);
            if(buscado == null)
            {
                retorno.Mensaje = "El usuario que intenta actualizar no existe.";
                retorno.Codigo = CodigosConstantes.ElementoNoEncontrado;
                retorno.Entidad = null;
                return retorno;
            }
            buscado.Bloqueado = estado;
            buscado = _repositorioUsuario.Modificar(buscado);
            if (!estado)
            {
                retorno.Mensaje = "Usuario " + buscado.Id + " autorizado con exito.";
            }
            else
            {
                retorno.Mensaje = "Usuario " + buscado.Id + " bloqueado con exito.";
            }
            retorno.Codigo = CodigosConstantes.TransmisionOK;
            retorno.Entidad = buscado;
            return retorno;
        }
        internal ResultadoOperacion VerUsuariosMasSeguidos()
        {
            ResultadoOperacion retorno = new ResultadoOperacion()
            {
                Codigo = CodigosConstantes.TransmisionOK,
                Mensaje = "",
                Entidad = _repositorioUsuario.VerUsuariosOrdenadosPorSeguidores()
            };

            return retorno;
        }

        internal ResultadoOperacion AsociarFoto(Usuario usuarioConectado, string nombre)
        {
            usuarioConectado.NombreDeFoto = nombre;
            ResultadoOperacion retorno = new ResultadoOperacion();
            if (nombre != null)
            {
                retorno.Codigo = CodigosConstantes.TransmisionOK;
                retorno.Mensaje = $"Foto asociada con éxito al perfil del usuario {usuarioConectado.NombreUsuario}";
                retorno.Tipo = "Foto";
            }
            else
            {
                retorno.Codigo = CodigosConstantes.Error;
                retorno.Mensaje = $"No fue posible completar la asociación";
            }
            return retorno;
        }

        internal ResultadoOperacion BusquedaUsuario(Usuario usuario, bool incluyente)
        {
            ResultadoOperacion retorno = new ResultadoOperacion()
            {
                Codigo = CodigosConstantes.TransmisionOK,
                Entidad = _repositorioUsuario.BuscarUsuarioPorCondicion(usuario, incluyente)
            };
            List<Usuario> usus = (List<Usuario>)retorno.Entidad;
            retorno.Mensaje = $"Se han encontrado {usus.Count()} usuarios en su búsqueda.";
            return retorno;
        }

        internal ResultadoOperacion SeguirUsuario(int idSeguido, int idSeguidor)
        {
            ResultadoOperacion retorno = new ResultadoOperacion()
            {
                Codigo = CodigosConstantes.TransmisionOK,
                Entidad = null,
                Mensaje = "El usuario fue agregado a la lista de seguidos."
            };
            if (idSeguido == idSeguidor)
            {
                retorno.Mensaje = "No puede seguirse a si mismo";
                retorno.Codigo = CodigosConstantes.ArgumentoNoValido;
                return retorno;
            }
            Usuario seguido = _repositorioUsuario.BuscarPorId(idSeguido);

            if (seguido == null)
            {
                retorno.Mensaje = "El usuario que desea seguir no existe.";
                retorno.Codigo = CodigosConstantes.ElementoNoEncontrado;
                return retorno;
            }

            Usuario seguidor = _repositorioUsuario.BuscarPorId(idSeguidor);

            if (seguido.Seguidores.Contains(seguidor))
            {
                retorno.Mensaje = "El usuario ya estaba en la lista de usuarios.";
                return retorno;
            }

            seguido.Seguidores.Add(seguidor);

            seguidor.Seguidos.Add(seguido);

            _repositorioUsuario.Modificar(seguido);
            _repositorioUsuario.Modificar(seguidor);

            return retorno;
        }

        internal ResultadoOperacion VerPerfilUsuario(int idUsu)
        {
            ResultadoOperacion retorno = new ResultadoOperacion()
            {
                Codigo = CodigosConstantes.TransmisionOK,
                Mensaje = "",
                Entidad = _repositorioUsuario.BuscarPorId(idUsu)
            };

            if (retorno.Entidad == null)
            {
                retorno.Codigo = CodigosConstantes.ElementoNoEncontrado;
                retorno.Mensaje = "El usuario no existe.";
            }

            return retorno;
        }

        internal List<int> ObtenerSeguidores(int id)
        {
            List<int> seguidores = new List<int>();
            Usuario seguido = _repositorioUsuario.BuscarPorId(id);
            foreach(Usuario seguidor in seguido.Seguidores)
            {
                seguidores.Add(seguidor.Id);
            }
            return seguidores;
        }

        internal List<Usuario> ObtenerUsuariosMasActivos(List<Chip> chips)
        {
            return _repositorioUsuario.ObtenerUsuariosMasActivos(chips, _cantUsuarios);
        }
    }
}
