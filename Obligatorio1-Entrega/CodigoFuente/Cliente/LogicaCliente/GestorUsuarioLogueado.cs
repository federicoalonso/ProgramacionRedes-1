using Cliente.Impresion;
using Servicios;
using Servicios.DTOs;
using Servicios.Excepciones;
using Servicios.Interfaces;
using Servicios.Protocolo;
using System.Text;

namespace Cliente.LogicaCliente
{
    internal class GestorUsuarioLogueado
    {
        static readonly EstadoCliente estadoCliente = EstadoCliente.ObtenerInstancia();
        private const int LargoMaximoCuerpoChip = 280;
        private const int LargoMinimoCuerpoChip = 0;
        private const int CantidadMínimaDeFotos = 0;
        private const int CantidadMaximaDeFotos = 3;

        internal static void VerUsuariosDelSistema(ClienteServidor conexion)
        {
            try
            {
                Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.VerUsuarios);
                conexion.EnviarTransmision(t);
            }
            catch (Exception ex)
            {
                GestorImpresion.Imprimir("Error al intentar ver usuarios.");
                GestorLog.EscribirLog(ex.Message);
            }
        }

        internal static void VerPerfilDeUsuario(ClienteServidor conexion)
        {
            bool listo = false;
            int idUsu = -1;
                      

            while (!listo)
            {
                try
                {
                    PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
                    pantallaBloqueada.Imprimir("Ingrese el id de usuario a ver:");
                    pantallaBloqueada.Imprimir("Ingrese 0 para salir");
                    idUsu = Convert.ToInt32(Console.ReadLine());
                    listo = true;
                }
                catch (ArgumentException ex)
                {
                    GestorImpresion.Imprimir("Argumento no válido.");
                }
                catch (Exception ex)
                {
                    GestorImpresion.Imprimir("Error al intentar ver perfil de usuario.");
                    GestorLog.EscribirLog(ex.Message);
                }
            }

            if (idUsu > 0)
            {
                Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.VerPerfil, idUsu.ToString());
                conexion.EnviarTransmision(t);
            }
        }

        internal static void BuscarUsuario(ClienteServidor conexion)
        {
            BusquedaDTO busqueda = new BusquedaDTO();
            PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
            pantallaBloqueada.Imprimir("\n\nBUSQUEDA DE USUARIO: \n");
            busqueda.Inclusivo = SeleccionarOpcionIncluyente(pantallaBloqueada);
            pantallaBloqueada.Imprimir("Ingrese el nombre de usuario:");
            busqueda.NombreUsuario = Console.ReadLine();
            pantallaBloqueada.Imprimir("Ingrese el nombre real:");
            busqueda.NombreReal = Console.ReadLine();
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.BuscarUsuario, busqueda.ToString());
            conexion.EnviarTransmision(t);
        }

        internal static void ResponderChip(ClienteServidor conexion)
        {
            ChipDTO chipDTO = new ChipDTO();
            bool correcto = false;
            while (!correcto)
            {
                try
                {
                    PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
                    pantallaBloqueada.Imprimir("Ingrese el Id del chip a responder: \n");
                    chipDTO.ChipPadreId = Convert.ToInt32(Console.ReadLine());
                    pantallaBloqueada.Imprimir("Ingrese contenido del chip: \n");
                    chipDTO.Cuerpo = Console.ReadLine();
                    correcto = true;
                }
                catch (ArgumentException ex)
                {
                    GestorImpresion.Imprimir("Argumento no válido.");
                }
                catch (Exception ex)
                {
                    GestorImpresion.Imprimir("Error al intentar responder un chip.");
                    GestorLog.EscribirLog(ex.Message);
                }
            }
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.ResponderChip, chipDTO.ToString());
            conexion.EnviarTransmision(t);
        }

        internal static void CrearChip(ClienteServidor conexion)
        {
            ChipDTO chipDTO = ObtenerDetallesDelChip();
            chipDTO.ChipPadreId = -1;
            if (chipDTO.CantidadDeFotos > 0)
            {
                try
                {
                    IGestorEnvioArchivos gestor = FabricaInterfaces.FabricaGestorEnvioArchivo();
                    string metadata;
                    for (int indice = 0; indice < chipDTO.CantidadDeFotos; indice++)
                    {
                        metadata = gestor.ObtenerMetadataFoto(chipDTO.rutasFotos[indice]);
                        if (metadata != null)
                        {
                            MetadataFotoDTO metadataDTO = new MetadataFotoDTO();
                            metadataDTO.ToObjeto(metadata);
                            chipDTO.metadataFotos.Add(metadataDTO);
                        }
                        else
                        {
                            lock (estadoCliente.ConsolaCandado)
                            {
                                GestorImpresion.Imprimir("Error al cargar la información del archivo, intente nuevamente.");
                            }
                        }

                    }
                    EnviarChip(conexion, chipDTO);
                    EnviarFotosChip(conexion, chipDTO);
                }
                catch (ExcepcionEnAccesoAArchivo excepcion)
                {
                    GestorImpresion.Imprimir(excepcion.mensaje);
                    GestorLog.EscribirLog(excepcion.Message);

                }
                catch (ExcepcionEnLecturaDeArchivoDuranteEnvio excepcion)
                {
                    GestorImpresion.Imprimir(excepcion.mensaje);
                    GestorLog.EscribirLog(excepcion.Message);
                    estadoCliente._conexion.Desconectarse();
                }
            }
            else
            {
                EnviarChip(conexion, chipDTO);
            }
        }

        private static ChipDTO ObtenerDetallesDelChip()
        {
            ChipDTO chipDTO = new ChipDTO();
            PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
            pantallaBloqueada.Imprimir("Ingrese contenido del chip: \n");
            chipDTO.Cuerpo = Console.ReadLine();
            pantallaBloqueada.Imprimir("Ingrese cantidad de fotos del chip: \n");
            bool valorValido = false;
            string cantidadFotos;
            int cantidad = -1;
            while (!valorValido)
            {
                try
                {
                    cantidadFotos = Console.ReadLine();
                    cantidad = Convert.ToInt32(cantidadFotos);
                    if (cantidad > CantidadMaximaDeFotos)
                    {
                        pantallaBloqueada.Imprimir($"Máximo de fotos permitida: {CantidadMaximaDeFotos}");
                    }
                    else if (cantidad < CantidadMínimaDeFotos)
                    {
                        pantallaBloqueada.Imprimir($"Mínimo de fotos permitida: {CantidadMínimaDeFotos}");
                    }
                    else
                    {
                        valorValido = true;
                    }
                }
                catch (FormatException excepcionFormato)
                {
                    pantallaBloqueada.Imprimir("Ingrese un valor númerico");
                }
                catch (OverflowException)
                {
                    pantallaBloqueada.Imprimir($"Ingrese un valor númerico entre {CantidadMínimaDeFotos} y {CantidadMaximaDeFotos}");
                }
            }
            string ruta;
            chipDTO.CantidadDeFotos = cantidad;
            for (int i = 1; i <= cantidad; i++)
            {
                pantallaBloqueada.Imprimir($"Ingrese ruta de la foto número { i }:");
                ruta = Console.ReadLine();
                chipDTO.rutasFotos.Add(ruta);
            }
            return chipDTO;
        }

        internal static void Logout(ClienteServidor conexion)
        {
            UsuarioDTO usu = estadoCliente._usuario;
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.Logout, usu.ToString());
            conexion.EnviarTransmision(t);
        }

        internal static void BuscarChipsPorPalabra(ClienteServidor conexion)
        {
            string cadenaBuscada;
            GestorImpresion.Imprimir("Escriba la cadena a buscar entre los chips");
            cadenaBuscada = Console.ReadLine();     
            BusquedaChipsDTO parametrosDTO = new BusquedaChipsDTO();
            parametrosDTO.CadenaBuscada = cadenaBuscada;
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.BuscarChipsPorPalabra, parametrosDTO.ToString());
            conexion.EnviarTransmision(t);
        }

        private static void EnviarFotosChip(ClienteServidor conexion, ChipDTO chipDTO)
        {
            try
            {
                int fotosEnviadas = 0;
                while (fotosEnviadas < chipDTO.CantidadDeFotos)
                {
                    conexion.EnviarArchivo(chipDTO.rutasFotos[fotosEnviadas]);
                    fotosEnviadas++;
                }
            }
            catch (ExcepcionEnManejadorFileStream excepcion)
            {
                GestorLog.EscribirLog(excepcion.Message);
                GestorImpresion.Imprimir(excepcion.mensaje);
            }
        }

        private static void EnviarChip(ClienteServidor conexion, ChipDTO chipDTO)
        {
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.CrearChip, chipDTO.ToString());
            conexion.EnviarTransmision(t);
        }

        private static bool SeleccionarOpcionIncluyente(PantallaCliente pantallaBloqueada)
        {
            bool incluyente = false;
            bool correcto = false;
            while (!correcto)
            {
                try
                {
                    pantallaBloqueada.Imprimir("SELECCIONE UNA OPCION: \n");
                    pantallaBloqueada.Imprimir("1 - Busqueda incluyente");
                    pantallaBloqueada.Imprimir("2 - Buscar excluyente");

                    int numInc = Convert.ToInt32(Console.ReadLine());

                    switch (numInc)
                    {
                        case 1:
                            incluyente = true;
                            correcto = true;
                            break;
                        case 2:
                            incluyente = false;
                            correcto = true;
                            break;
                        default:
                            pantallaBloqueada.Imprimir("Seleccione una opcion valida.");
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    pantallaBloqueada.Imprimir("Argumento no válido.");
                }
                catch (Exception excepcion)
                {
                    pantallaBloqueada.Imprimir("Ocurrió un error inesperado, intente nuevamente");
                    GestorLog.EscribirLog(excepcion.Message);
                }
            }
            return incluyente;
        }

        internal static void AsociarFotoAlPerfil(ClienteServidor conexion)
        {
            string ruta;
            string mensajeDeError;
            GestorImpresion.Imprimir("Indique la ruta del archivo");
            ruta = Console.ReadLine();
            IGestorEnvioArchivos gestor = FabricaInterfaces.FabricaGestorEnvioArchivo();
            try
            {
                string metadataFoto = gestor.ObtenerMetadataFoto(ruta);
                if (metadataFoto != null)
                {
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
                GestorLog.EscribirLog(excepcion.Message);
            }
            catch (ExcepcionEnManejadorFileStream excepcion)
            {
                GestorImpresion.Imprimir(excepcion.mensaje);
                GestorLog.EscribirLog(excepcion.Message);
            }
        }

        internal static void SeguirUsuario(ClienteServidor conexion)
        {
            bool correcto = false;
            int idSeguido = -1;
            while (!correcto)
            {
                PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
                try
            {
                    pantallaBloqueada.Imprimir("Ingrese el ID del usuario a seguir:");
                    idSeguido = Convert.ToInt32(Console.ReadLine());
                    correcto = true;
                }
                catch (ArgumentException ex)
                {
                    pantallaBloqueada.Imprimir("Argumento no válido.");
                }
                catch (Exception ex)
                {
                    pantallaBloqueada.Imprimir("Error al intentar seguir usuario.");
                    GestorLog.EscribirLog(ex.Message);
                }
            }            
            SeguirUsuarioDTO relacion = new SeguirUsuarioDTO();
            relacion.Seguido = idSeguido;
            int idPropio = estadoCliente._usuario.Id;
            relacion.Seguidor = idPropio;
            Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.SeguirUsuario, relacion.ToString());
            conexion.EnviarTransmision(t);
        }
    }
}
