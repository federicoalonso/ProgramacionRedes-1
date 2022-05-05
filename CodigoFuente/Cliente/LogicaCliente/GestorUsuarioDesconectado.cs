using Cliente.Impresion;
using Servicios;
using Servicios.DTOs;
using Servicios.Excepciones;
using Servicios.Interfaces;
using Servicios.Protocolo;
using System.Text;

namespace Cliente.LogicaCliente
{
    internal static class GestorUsuarioDesconectado
    {
        static readonly EstadoCliente estadoCliente = EstadoCliente.ObtenerInstancia();
        private const string perfilConFoto = "s";
        private const string perfilSinFoto = "n";

        internal static void LoginUsuario(ClienteServidor conexion)
        {
            PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
            LoginDTO usuario = new LoginDTO();
            pantallaBloqueada.Imprimir("Escriba el nombre de usuario:");
            usuario.NombreDeUsuario = Console.ReadLine();
            pantallaBloqueada.Imprimir("Escriba la contraseña:");
            usuario.Password = Console.ReadLine();
            
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.Login, usuario.ToString());
            conexion.EnviarTransmision(t);
        }

        internal static void AltaDeUsuario(ClienteServidor conexion)
        {
            bool valido;
            UsuarioDTO usuario = new UsuarioDTO();
            string conFoto;
            PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
            pantallaBloqueada.Imprimir("Escriba el nombre real:");
            usuario.NombreReal = Console.ReadLine();
            pantallaBloqueada.Imprimir("Escriba el nombre de usuario:");
            usuario.NombreUsuario = Console.ReadLine();
            pantallaBloqueada.Imprimir("Escriba la contraseña:");
            usuario.Contrasenia = Console.ReadLine();
            pantallaBloqueada.Imprimir("Desea adjuntar foto a su perfil? S o N:");
            conFoto = Console.ReadLine();
            valido = conFoto.Trim().ToLower().Equals(perfilConFoto) || conFoto.Trim().ToLower().Equals(perfilSinFoto);
            while (!valido)
            {
                pantallaBloqueada.Imprimir("Seleccione una opcion válida.");
                conFoto = Console.ReadLine();
                if (conFoto.Trim().ToLower().Equals(perfilConFoto) || conFoto.Trim().ToLower().Equals(perfilSinFoto))
                {
                    valido = true;
                }
            }
            switch (conFoto.Trim().ToLower())
            {
                case "s":
                    AltaDeUsuarioConFoto(conexion, usuario);
                    break;
                case "n":
                    SolicitarAltaUsuario(conexion, usuario);
                    break;
            }
        }

        private static void AltaDeUsuarioConFoto(ClienteServidor conexion, UsuarioDTO usuario)
        {
            string ruta;
            string mensajeDeError;
            GestorImpresion.Imprimir("Indique la ruta del archivo");
            ruta = Console.ReadLine();
            IGestorEnvioArchivos gestor = FabricaInterfaces.FabricaGestorEnvioArchivo();
            try
            {
                string metadataFoto = gestor.ObtenerMetadataFoto(ruta);
                ManejadorFileStram.VerificarAccesibilidadDeArchivo(ruta);
                if (metadataFoto != null)
                {
                    SolicitarAltaUsuario(conexion, usuario);
                    Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.AsociarFotoAlPerfil, metadataFoto);
                    conexion.EnviarTransmision(t);
                    conexion.EnviarArchivo(ruta);
                }
                else
                {
                    GestorImpresion.Imprimir("Error al cargar la información del archivo, intente nuevamente.");
                }
            }
            catch (ExcepcionEnAccesoAArchivo excepcion)
            {
                GestorImpresion.Imprimir(excepcion.mensaje);
                GestorLog.EscribirLog(excepcion.mensaje);
            }
            catch (ExcepcionEnManejadorFileStream excepcion)
            {
                GestorImpresion.Imprimir(excepcion.mensaje);
                GestorLog.EscribirLog(excepcion.mensaje);
            }
        }

        private static void SolicitarAltaUsuario(ClienteServidor conexion, UsuarioDTO usuario)
        {
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.AltaUsuario, usuario.ToString());
            conexion.EnviarTransmision(t);
        }

        internal static void ConectarseAlServidor(ClienteServidor conexion)
        {
            conexion.ConectarseAlServidor();
        }

        internal static void Desconectarse(ClienteServidor conexion)
        {
            conexion.Desconectarse();
        }
    }
}
