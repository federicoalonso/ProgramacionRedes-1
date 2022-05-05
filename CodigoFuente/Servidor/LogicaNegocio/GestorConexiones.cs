using Servicios;
using Servicios.Conexiones;
using Servicios.DTOs;
using Servicios.Excepciones;
using Servicios.Interfaces;
using Servicios.Protocolo;
using Servidor.Dominio;
using Servidor.Pantalla;
using Servidor.Repositorio;
using System.Net.Sockets;
using System.Text;

namespace Servidor.LogicaNegocio
{
    internal class GestorConexiones
    {
        private static RepositorioConexion _repositorioConexiones = RepositorioConexion.ObtenerInstancia();

        internal static void GestionarCliente(Socket cliente)
        {
            IConexion _conexion = FabricaInterfaces.FabricaConexion();
            _conexion.SocketHelp = FabricaInterfaces.FabricaSocketHelper(cliente);
            Thread hilo = new Thread(() => EscuchandoCliente(_conexion));
            _conexion = _repositorioConexiones.AltaConexion(_conexion);
            hilo.Start();
        }

        internal bool VerificarUsuarioConectado(Usuario usuario)
        {
            if (usuario == null)
                return false;
            IConexion conexion = _repositorioConexiones.BuscarPorUsuario(usuario);
            bool retorno = false;
            if(conexion != null) 
                retorno = true;
            return retorno;
        }

        private static void EscuchandoCliente(IConexion conexion)
        {
            bool conectado = true;
            GestorImpresion.Imprimir("Conectado el cliente " + conexion.Id);
            GestorLog.EscribirLog("Conectado el cliente " + conexion.Id);

            try
            {
                while (conectado)
                {
                    Transmision t = conexion.EscucharMensajes(HeaderConstantes.Request.Length + HeaderConstantes.ComandosLength + HeaderConstantes.DataLength);
                    GestorLog.EscribirLog($"Cliente {conexion.Id} dice {t.BodyT}");
                    GestionarTransmision(t, conexion);
                 }
            }
            catch (SocketException ex)
            {
                GestorImpresion.Imprimir("El cliente " + conexion.Id + " cerró la conexión");
                GestorLog.EscribirLog("El cliente " + conexion.Id + " cerró la conexión");
                GestorLog.EscribirLog($"Socket error: {ex.SocketErrorCode} / {ex.ErrorCode} msg: {ex.Message}");
                conectado = false;
                _repositorioConexiones.BajaConexion(conexion);
            }
            catch (Exception ex)
            {
                conectado = false;
                GestorLog.EscribirLog(ex.Message);
                GestorImpresion.Imprimir("Error de conexión con el cliente "+ conexion.Id);
                _repositorioConexiones.BajaConexion(conexion);
            }
        }

        private static void GestionarTransmision(Transmision req, IConexion conexion)
        {
            try
            {
                int comando = req.HeaderT.IComando;
                switch (comando)
                {
                    case ComandosConstantes.Login:
                        LoginUsario(req, conexion);
                        break;
                    case ComandosConstantes.AltaUsuario:
                        AltaUsuario(req, conexion);
                        break;
                    case ComandosConstantes.VerUsuarios:
                        VerUsuarios(req, conexion);
                        break;
                    case ComandosConstantes.VerPerfil:
                        VerPerfil(req, conexion);
                        break;
                    case ComandosConstantes.BuscarUsuario:
                        BuscarUsuarios(req, conexion);
                        break;
                    case ComandosConstantes.SeguirUsuario:
                        SeguirUsuario(req, conexion);
                        break;
                    case ComandosConstantes.AsociarFotoAlPerfil:
                        AsociarFotoAPerfil(req, conexion);
                        break;
                    case ComandosConstantes.CrearChip:
                        CrearChip(req, conexion);
                        break;
                    case ComandosConstantes.BuscarChipsPorPalabra:
                        BuscarChipsPorPalabra(req, conexion);
                        break;
                    case ComandosConstantes.Logout:
                        Logout(req, conexion);
                        break;
                    case ComandosConstantes.ResponderChip:
                        ResponderChip(req, conexion);
                        break;
                    default:
                        break;
                }
            }
            catch (SocketException ex)
            {
                StringBuilder salida = new StringBuilder();
                salida.AppendLine("El cliente " + conexion.Id + " cerró la conexión");
                salida.AppendLine(($"Socket error: {ex.SocketErrorCode} / {ex.ErrorCode} msg: {ex.Message}"));
                GestorImpresion.Imprimir(salida.ToString());
                GestorLog.EscribirLog(salida.ToString());
                _repositorioConexiones.BajaConexion(conexion);
            }
            catch (Exception ex)
            {
                GestorImpresion.Imprimir("Error al recibir transmisión.");
                GestorLog.EscribirLog(ex.Message);
                _repositorioConexiones.BajaConexion(conexion);
            }
        }

        private static void ResponderChip(Transmision req, IConexion conexion)
        {
            ChipDTO chipDTO = new ChipDTO();
            chipDTO.ToObjeto(req.BodyT);
            Chip chip = new Chip()
            {
                CantidadDeFotos = 0,
                ChipPadreId = chipDTO.ChipPadreId,
                Cuerpo = chipDTO.Cuerpo,
            };
            Usuario usuarioConectado = (Usuario)conexion.EntidadConectada;
            chip.UsuarioId = usuarioConectado.Id;
            GestorChip gestor = new GestorChip();
            ResultadoOperacion resultado = gestor.AltaChip(chip);
            Chip res = (Chip)resultado.Entidad;
            if (res != null)
            {
                chipDTO.Id = res.Id;
                chipDTO.CantidadDeFotos = 0;
                chipDTO.UsuarioId = res.UsuarioId;
                resultado.Entidad = chipDTO;
                resultado.Tipo = "ChipDTO";
                NotificarChipASeguidores((Usuario)conexion.EntidadConectada, chipDTO);
            }
            else
            {
                resultado.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.ResponderChip, resultado.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void Logout(Transmision req, IConexion conexion)
        {
            GestorUsuario _gest = new GestorUsuario();
            UsuarioDTO usuario = new UsuarioDTO();
            usuario.ToObjeto(req.BodyT);
            _repositorioConexiones.Logout(usuario.Id);
            ResultadoOperacion res = new ResultadoOperacion();
            res.Mensaje = "Logout efectuado con éxito.";
            res.Entidad = null;
            res.Codigo = CodigosConstantes.TransmisionOK;
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.Logout, res.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void BuscarChipsPorPalabra(Transmision req, IConexion conexion)
        {
            BusquedaChipsDTO busquedaChipsDTO = new BusquedaChipsDTO();
            busquedaChipsDTO.ToObjeto(req.BodyT);
            GestorChip gestor = new GestorChip();
            ResultadoOperacion resultado = gestor.BuscarChipsPorPalabra(busquedaChipsDTO);
            if (resultado.Entidad != null)
            {
                resultado.Tipo = "ListChipDTO";
                List<ChipDTO> chipsDTO = new List<ChipDTO>();
                List<Chip> chips = (List<Chip>)resultado.Entidad;

                foreach (Chip chip in chips)
                {
                    ChipDTO chipDTO = new ChipDTO()
                    {
                        CantidadDeFotos = chip.CantidadDeFotos,
                        ChipPadreId = chip.ChipPadreId,
                        Cuerpo = chip.Cuerpo,
                        Id = chip.Id,
                        metadataFotos = chip.metadataFotos,
                        UsuarioId = chip.UsuarioId,
                    };  
                    chipsDTO.Add(chipDTO);
                }
                resultado.Entidad = chipsDTO;
                resultado.Mensaje = chipsDTO.Count() + " chips cumplen con su búsqueda.";
            }
            else
            {
                resultado.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.BuscarChipsPorPalabra, resultado.ToString());
            ResponderAlCliente(t, conexion);

        }

        internal void DesconectarUsuario(int id)
        {
            _repositorioConexiones.DesconectarUsuario(id);
        }

        private static void CrearChip(Transmision req, IConexion conexion)
        {
            try
            {
                ChipDTO chipDTO = new ChipDTO();
                chipDTO.ToObjeto(req.BodyT);
                Chip chip = new Chip()
                {
                    CantidadDeFotos = chipDTO.CantidadDeFotos,
                    ChipPadreId = chipDTO.ChipPadreId,
                    Cuerpo = chipDTO.Cuerpo,
                    metadataFotos = chipDTO.metadataFotos,
                };
                IGestorEnvioArchivos _gestorEnvioArchivos = FabricaInterfaces.FabricaGestorEnvioArchivo();
                int indice = 0;
                while (indice < chip.CantidadDeFotos)
                {
                    // Lanza excepcionEnManejadorFileStream la excepcion ExcepcionEnLecturaDeArchivoDuranteEnvio se maneja desde afuera y el catch cierra la conexion
                    _gestorEnvioArchivos.RecibirArchvivo(chipDTO.metadataFotos[indice], conexion.SocketHelp);
                    indice++;
                }
                Usuario usuarioConectado = (Usuario)conexion.EntidadConectada;
                chip.UsuarioId = usuarioConectado.Id;
                GestorChip gestor = new GestorChip();
                ResultadoOperacion resultado = gestor.AltaChip(chip);
                Chip res = (Chip)resultado.Entidad;
                if (res != null)
                {
                    chipDTO.Id = res.Id;
                    chipDTO.UsuarioId = res.UsuarioId;
                    resultado.Entidad = chipDTO;
                    resultado.Tipo = "ChipDTO";
                    NotificarChipASeguidores((Usuario)conexion.EntidadConectada, chipDTO);
                }
                else
                {
                    resultado.Tipo = "Error";
                }
                Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.CrearChip, resultado.ToString());
                ResponderAlCliente(t, conexion);
            }
            catch (ExcepcionEnManejadorFileStream excepcion)
            {
                GestorImpresion.Imprimir(excepcion.mensaje);
                GestorLog.EscribirLog(excepcion.mensaje);
            }
            catch (ExcepcionEnLecturaDeArchivoDuranteEnvio excepcion)
            {
                GestorImpresion.Imprimir(excepcion.mensaje);
                GestorLog.EscribirLog(excepcion.mensaje);
                // Cerrar conexion porque hubo un error en la comunicacion
                _repositorioConexiones.BajaConexion(conexion);
            }
        }

        private static void NotificarChipASeguidores(Usuario usuario, ChipDTO chip)
        {
            ResultadoOperacion resultado = new ResultadoOperacion();
            resultado.Mensaje = "Nuevo mensaje de un usuario que sigue.";
            resultado.Tipo = "ChipDTO";
            resultado.Entidad = chip;
            resultado.Codigo = CodigosConstantes.TransmisionOK;
            GestorUsuario gestorUsuario = new GestorUsuario();
            List<int> seguidoresId = gestorUsuario.ObtenerSeguidores(usuario.Id);
            foreach(int id in seguidoresId)
            {
                IConexion conexion = _repositorioConexiones.ObtenerConexionUsuario(id);
                if(conexion != null)
                {
                    Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.NotificarChipSeguido, resultado.ToString());
                    conexion.EnviarTransmision(t);
                }
            }
        }

        private static void AsociarFotoAPerfil(Transmision req, IConexion conexion)
        {
            try { 
                MetadataFotoDTO dtoFoto = new MetadataFotoDTO();
                dtoFoto.ToObjeto(req.BodyT);
                IGestorEnvioArchivos _gestorEnvioArchivos = FabricaInterfaces.FabricaGestorEnvioArchivo();
                _gestorEnvioArchivos.RecibirArchvivo(dtoFoto, conexion.SocketHelp);
                GestorUsuario gestor = new GestorUsuario();
                ResultadoOperacion resultado = gestor.AsociarFoto((Usuario)conexion.EntidadConectada, dtoFoto.Nombre);
                Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.AsociarFotoAlPerfil, resultado.ToString());
                ResponderAlCliente(t, conexion);
            }
            catch (ExcepcionEnManejadorFileStream excepcion)
            {
                GestorImpresion.Imprimir(excepcion.mensaje);
                GestorLog.EscribirLog(excepcion.mensaje);
            }
            catch (ExcepcionEnLecturaDeArchivoDuranteEnvio excepcion)
            {
                GestorImpresion.Imprimir(excepcion.mensaje);
                GestorLog.EscribirLog(excepcion.mensaje);
                // Cerrar conexion porque hubo un error en la comunicacion
                _repositorioConexiones.BajaConexion(conexion);
            }
        }

        private static void SeguirUsuario(Transmision req, IConexion conexion)
        {
            GestorUsuario _gest = new GestorUsuario();
            SeguirUsuarioDTO relacion = new SeguirUsuarioDTO();
            relacion.ToObjeto(req.BodyT);
            ResultadoOperacion res = _gest.SeguirUsuario(relacion.Seguido, relacion.Seguidor);
            if (res.Codigo < CodigosConstantes.Error)
            {
                res.Tipo = "OKRespuesta";
            }
            else
            {
                res.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.SeguirUsuario, res.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void BuscarUsuarios(Transmision req, IConexion conexion)
        {
            GestorUsuario _gestUsuario = new GestorUsuario();
            GestorChip _gestChip = new GestorChip();
            Usuario usuario = new Usuario();
            BusquedaDTO busquedaDTO = new BusquedaDTO();
            busquedaDTO.ToObjeto(req.BodyT);
            usuario.NombreUsuario = busquedaDTO.NombreUsuario;
            usuario.NombreReal = busquedaDTO.NombreReal;
            ResultadoOperacion res = _gestUsuario.BusquedaUsuario(usuario, busquedaDTO.Inclusivo);
            if (res.Entidad != null)
            {
                res.Tipo = "ListUsuarioDTO";
                List<UsuarioDTO> usuariosDTO = new List<UsuarioDTO>();
                List<Usuario> usuarios = (List<Usuario>)res.Entidad;

                foreach (Usuario usu in usuarios)
                {
                    UsuarioDTO usuarioDTO = new UsuarioDTO();
                    usuarioDTO.Id = usu.Id;
                    //usuarioDTO.Contrasenia = usu.Contrasenia;
                    usuarioDTO.NombreUsuario = usu.NombreUsuario;
                    usuarioDTO.NombreReal = usu.NombreReal;
                    usuarioDTO.Seguidores = usu.Seguidores.Count();
                    usuarioDTO.Seguidos = usu.Seguidos.Count();
                    usuarioDTO.CantidadChips = _gestChip.CantidadChipsPorUsuario(usu.Id);
                    //List<Chip> chips = _gestChip.ChipsPorUsuario(usu.Id);
                    //foreach(Chip ch in chips)
                    //{
                    //    ChipDTO chipDTO = new ChipDTO();
                    //    chipDTO.Id = ch.Id;
                    //    chipDTO.ChipPadreId = ch.ChipPadreId;
                    //    chipDTO.Cuerpo = ch.Cuerpo;
                    //    chipDTO.UsuarioId = ch.UsuarioId;
                    //    chipDTO.CantidadDeFotos = ch.CantidadDeFotos;
                    //    chipDTO.metadataFotos = ch.metadataFotos;
                    //    usuarioDTO.Chips.Add(chipDTO);
                    //}
                    usuariosDTO.Add(usuarioDTO);
                }
                res.Entidad = usuariosDTO;
            }
            else
            {
                res.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.BuscarUsuario, res.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void VerPerfil(Transmision req, IConexion conexion)
        {
            GestorUsuario _gest = new GestorUsuario();
            GestorChip _gestChip = new GestorChip();
            int idUsuario = int.Parse(req.BodyT);

            ResultadoOperacion res = _gest.VerPerfilUsuario(idUsuario);
            if (res.Entidad != null)
            {
                res.Tipo = "UsuarioDTO";
                UsuarioDTO usuarioDTO = new UsuarioDTO();
                Usuario usu = (Usuario)res.Entidad;
                usuarioDTO.Id = usu.Id;
                usuarioDTO.NombreUsuario = usu.NombreUsuario;
                usuarioDTO.NombreReal = usu.NombreReal;
                usuarioDTO.Seguidores = usu.Seguidores.Count();
                usuarioDTO.Seguidos = usu.Seguidos.Count();
                usuarioDTO.NombreDeFoto = usu.NombreDeFoto;
                usuarioDTO.CantidadChips = _gestChip.CantidadChipsPorUsuario(usu.Id);
                List<Chip> chips = _gestChip.ChipsPorUsuario(usu.Id);
                foreach (Chip ch in chips)
                {
                    ChipDTO chipDTO = new ChipDTO();
                    chipDTO.Id = ch.Id;
                    chipDTO.ChipPadreId = ch.ChipPadreId;
                    chipDTO.Cuerpo = ch.Cuerpo;
                    chipDTO.UsuarioId = ch.UsuarioId;
                    chipDTO.CantidadDeFotos = ch.CantidadDeFotos;
                    chipDTO.metadataFotos = ch.metadataFotos;
                    usuarioDTO.Chips.Add(chipDTO);
                }
                res.Entidad = usuarioDTO;
            }
            else
            {
                res.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.VerPerfil, res.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void VerUsuarios(Transmision req, IConexion conexion)
        {
            GestorUsuario _gest = new GestorUsuario();
            GestorChip _gestChip = new GestorChip();

            ResultadoOperacion res = _gest.VerUsuarios();
            if (res.Entidad != null)
            {
                res.Tipo = "ListUsuarioDTO";
                List<UsuarioDTO>usuariosDTO = new List<UsuarioDTO>();
                List<Usuario>usuarios = (List<Usuario>)res.Entidad;

                foreach (Usuario usu in usuarios)
                {
                    UsuarioDTO usuarioDTO = new UsuarioDTO();
                    usuarioDTO.Id = usu.Id;
                    usuarioDTO.Contrasenia = usu.Contrasenia;
                    usuarioDTO.NombreUsuario = usu.NombreUsuario;
                    usuarioDTO.NombreReal = usu.NombreReal;
                    usuarioDTO.Seguidores = usu.Seguidores.Count();
                    usuarioDTO.Seguidos = usu.Seguidos.Count();
                    usuarioDTO.CantidadChips = _gestChip.CantidadChipsPorUsuario(usu.Id);
                    usuariosDTO.Add(usuarioDTO);
                }
                res.Entidad = usuariosDTO;
            }
            else
            {
                res.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.VerUsuarios, res.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void AltaUsuario(Transmision req, IConexion conexion)
        {
            GestorUsuario _gest = new GestorUsuario();
            Usuario usuario = new Usuario();
            UsuarioDTO usuDTO = new UsuarioDTO();
            usuDTO.ToObjeto(req.BodyT);
            usuario.NombreUsuario = usuDTO.NombreUsuario;
            usuario.NombreReal = usuDTO.NombreReal;
            usuario.Contrasenia = usuDTO.Contrasenia;
            ResultadoOperacion res = _gest.AltaUsuario(usuario);
            if (res.Entidad != null)
            {
                res.Tipo = "UsuarioDTO";
                UsuarioDTO usuarioDTO = new UsuarioDTO();
                Usuario usu = (Usuario)res.Entidad;
                usuarioDTO.Id = usu.Id;
                usuarioDTO.Contrasenia = usu.Contrasenia;
                usuarioDTO.NombreUsuario = usu.NombreUsuario;
                usuarioDTO.NombreReal = usu.NombreReal;
                res.Entidad = usuarioDTO;
                conexion.EntidadConectada = usuario;
            }
            else
            {
                res.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.AltaUsuario, res.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void LoginUsario(Transmision req, IConexion conexion)
        {
            conexion.EntidadConectada = null; // Para romper una posible asociacion de la creacion de un perfil nuevo
            GestorUsuario _gest = new GestorUsuario();
            Usuario usuario = new Usuario();
            LoginDTO loginDTO = new LoginDTO();
            loginDTO.ToObjeto(req.BodyT);
            usuario.NombreUsuario = loginDTO.NombreDeUsuario;
            usuario.Contrasenia = loginDTO.Password;
            ResultadoOperacion res = _gest.Login(usuario);
            if (res.Codigo < CodigosConstantes.Error)
            {
                res.Tipo = "UsuarioDTO";
                UsuarioDTO usuDTO = new UsuarioDTO();
                Usuario usu = (Usuario)res.Entidad;
                conexion.EntidadConectada = usu;
                usuDTO.Id = usu.Id;
                usuDTO.Contrasenia = usu.Contrasenia;
                usuDTO.NombreUsuario = usu.NombreUsuario;
                res.Entidad = usuDTO;
            }
            else
            {
                res.Tipo = "Error";
            }
            Transmision t = new Transmision(HeaderConstantes.Response, ComandosConstantes.Login, res.ToString());
            ResponderAlCliente(t, conexion);
        }

        private static void ResponderAlCliente(Transmision t, IConexion conexion)
        {
            conexion.EnviarTransmision(t);
        }
    }
}
