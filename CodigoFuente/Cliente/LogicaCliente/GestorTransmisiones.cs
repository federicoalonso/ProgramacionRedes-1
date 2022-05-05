using Cliente.Impresion;
using Cliente.Menus;
using Servicios;
using Servicios.DTOs;
using Servicios.Protocolo;
using System.Text;

namespace Cliente.LogicaCliente
{
    /*
     * Responsabilidades:
     * - Gestionar los mensajes que recibe del servidor
     */
    internal static class GestorTransmisiones
    {
        static readonly EstadoCliente estadoCliente = EstadoCliente.ObtenerInstancia();
        static readonly PantallaCliente pantalla = PantallaCliente.ObtenerInstancia();

        internal static void GestionarTodasLasTransmisiones()
        {
            ColaTransmision cola = ColaTransmision.ObtenerInstancia();
            Transmision? t;
            do
            {
                t = cola.PopTransmision();
                if(t != null)
                {
                    GestionarLaTransmision(t);
                }
            } while (t != null);

        }
        private static void GestionarLaTransmision(Transmision res)
        {
            ResultadoOperacion resultado = new ResultadoOperacion();
            resultado.ToObject(res.BodyT);
            int comando = res.HeaderT.IComando;

            if (resultado.Codigo >= CodigosConstantes.Error)
            {
                GestorImpresion.Imprimir(resultado.Mensaje);
                if (comando == ComandosConstantes.Logout)
                {
                    // Si el error es de logout, el otro hilo se finaliza, por lo que hay que iniciarlo nuevamente
                    MenuUsuarioLogueado appLog = new MenuUsuarioLogueado();
                    new Thread(() => appLog.Menu()).Start();
                }
                return;
            }

            GestorImpresion.Imprimir(resultado.Mensaje);

            switch (comando)
            {
                case ComandosConstantes.Login:
                    Login(resultado);
                    break;
                case ComandosConstantes.AltaUsuario:
                    break;
                case ComandosConstantes.VerUsuarios:
                    MostrarUsuarios(resultado);
                    break;
                case ComandosConstantes.VerPerfil:
                    MostrarPerfil(resultado);
                    break;
                case ComandosConstantes.BuscarUsuario:
                    BuscarUsuarios(resultado);
                    break;
                case ComandosConstantes.SeguirUsuario:
                    SeguirUsuario(resultado);
                    break;
                case ComandosConstantes.BuscarChipsPorPalabra:
                    BuscarChipsPorPalabra(resultado);
                    break;
                case ComandosConstantes.Logout:
                    Logout(resultado);
                    break;
                case ComandosConstantes.NotificarChipSeguido:
                    InformarNotificacion(resultado);
                    break;
                case ComandosConstantes.CrearChip:
                    MostrarChipCreado(resultado);
                    break;
                case ComandosConstantes.ResponderChip:
                    MostrarChipCreado(resultado);
                    break;
                case ComandosConstantes.AsociarFotoAlPerfil:
                    AsociarFotoAlPerfil(resultado);
                    break;
                default:
                    GestorImpresion.Imprimir("Tipo de transmisión no implementada.");
                    break;
            }
        }
        internal static void GestionarTransmision(Transmision res)
        {
            ColaTransmision cola = ColaTransmision.ObtenerInstancia();
            cola.PushTransmision(res);
        }

        private static void AsociarFotoAlPerfil(ResultadoOperacion resultado)
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("\nRESULTADO: \n");
            salida.AppendLine(resultado.Mensaje);
            GestorImpresion.Imprimir(salida.ToString());
        }

        private static void MostrarChipCreado(ResultadoOperacion resultado)
        {
            ChipDTO chipDTO = (ChipDTO)resultado.Entidad;
            List<ChipDTO> chips = new List<ChipDTO>();
            chips.Add(chipDTO);
            MostrarChips(chips);
        }

        private static void InformarNotificacion(ResultadoOperacion resultado)
        {
            ChipDTO chip = new ChipDTO();
            int respuesta = -1;
            List<ChipDTO> chips = new List<ChipDTO>();
            ChipDTO chipDTO = (ChipDTO)resultado.Entidad;
            chips.Add(chipDTO);
            MostrarChips(chips);
            bool correcto = false;
            while (!correcto)
            {
                PantallaCliente pantallaBloqueada = GestorImpresion.BloquearPantalla();
                try
                {
                    pantallaBloqueada.Imprimir("Desea responder el Chip? (1 para sí, 0 para no):");
                    respuesta = Convert.ToInt32(Console.ReadLine());
                    if (respuesta > -1 && respuesta < 2)
                    {
                        correcto = true;
                        if (respuesta == 1)
                        {
                            pantallaBloqueada.Imprimir("\nIngrese contenido de la respuesta: \n");
                            chip.Cuerpo = Console.ReadLine();
                            chip.ChipPadreId = chipDTO.Id;
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    pantallaBloqueada.Imprimir("Argumento no válido.");
                }
                catch (Exception ex)
                {
                    pantallaBloqueada.Imprimir(ex.Message);
                    GestorLog.EscribirLog(ex.Message);
                }
            }
            if (respuesta == 1)
            {
                chip.UsuarioId = estadoCliente._usuario.Id;
                Transmision t = new Transmision(HeaderConstantes.Request, ComandosConstantes.ResponderChip, chip.ToString()); ;
                estadoCliente._conexion.EnviarTransmision(t);
            }
        }

        private static void Logout(ResultadoOperacion resultado)
        {
            estadoCliente._usuario = null;
            MenuUsuarioDesconectado appDes = new MenuUsuarioDesconectado();
            new Thread(() => appDes.Menu()).Start();
        }

        private static void Login(ResultadoOperacion resultado)
        {
            estadoCliente._usuario = (UsuarioDTO)resultado.Entidad;
            MenuUsuarioLogueado appLog = new MenuUsuarioLogueado();
            new Thread(() => appLog.Menu()).Start();
        }

        private static void BuscarChipsPorPalabra(ResultadoOperacion resultado)
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("\n\nRESULTADO BUSQUEDA DE CHIPS POR PALABRA: \n");
            if (resultado.Entidad == null)
            {
                salida.AppendLine(resultado.Mensaje);
                lock (estadoCliente.ConsolaCandado)
                {
                    GestorImpresion.Imprimir(salida.ToString());
                }
            }
            else
            {
                List<ChipDTO> chips = (List<ChipDTO>)resultado.Entidad;
                MostrarChips(chips);
            }
        }

        private static void MostrarChips(List<ChipDTO> chips)
        {
            StringBuilder salida = new StringBuilder();
            int contadorChips = 1;
            foreach (ChipDTO chipDTO in chips)
            {
                salida.AppendLine($"Chip número {contadorChips}:");
                salida.AppendLine($"\tPublicado por: {chipDTO.UsuarioId}");
                salida.AppendLine($"\tChip Id: {chipDTO.Id}");
                if (chipDTO.ChipPadreId != -1)
                {
                    salida.Append($"\tEn respuesta a chip: {chipDTO.ChipPadreId}");
                }
                salida.AppendLine($"\tContenido: {chipDTO.Cuerpo}");
                salida.AppendLine($"\tCantidad De Fotos: {chipDTO.CantidadDeFotos}");
                int contadorFotos = 1;
                if (chipDTO.CantidadDeFotos > 0)
                {
                    List<MetadataFotoDTO> metadataFotos = chipDTO.metadataFotos;
                    foreach (MetadataFotoDTO metadata in metadataFotos)
                    {
                        salida.AppendLine($"\t\tFoto {contadorFotos}: ");
                        salida.AppendLine($"\t\t\tNombre Foto: {metadata.Nombre}");
                        salida.AppendLine($"\t\t\tNombre tamanio: {metadata.TamanioArchivo}");
                        contadorFotos++;
                    }
                }
                contadorChips++;
            }
            lock (estadoCliente.ConsolaCandado)
            {
                GestorImpresion.Imprimir(salida.ToString());
            }
        }

        private static void SeguirUsuario(ResultadoOperacion resultado)
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("\n\nRESULTADO: \n");
            salida.AppendLine(resultado.Mensaje);
            GestorImpresion.Imprimir(salida.ToString());
        }

        private static void BuscarUsuarios(ResultadoOperacion resultado)
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("\n\nRESULTADO BUSQUEDA DE USUARIO: \n");
            if (resultado.Entidad == null)
            {
                salida.AppendLine(resultado.Mensaje);
            }
            else
            {
                salida.AppendLine("\nID\tNOMBRE USUARIO\tNOMBRE REAL\tSEGUIDORES\tSEGUIDOS\tBLOQUEADO");
                List<UsuarioDTO> usus = (List<UsuarioDTO>)resultado.Entidad;
                foreach (UsuarioDTO usu in usus)
                {
                    salida.AppendLine(usu.Id + "\t" + usu.NombreUsuario + "\t" + usu.NombreReal + "\t" + usu.Seguidores + "\t\t" + usu.Seguidos);
                }
            }
            GestorImpresion.Imprimir(salida.ToString());
        }

        private static void MostrarPerfil(ResultadoOperacion resultado)
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("\n\nPERFIL DE USUARIO: \n");
            if (resultado.Entidad == null)
            {
                salida.AppendLine(resultado.Mensaje);
            }
            else
            {
                UsuarioDTO usu = (UsuarioDTO)resultado.Entidad;
                salida.AppendLine("ID\tNOMBRE USUARIO\tNOMBRE REAL\tFOTO\tSEGUIDORES\tSEGUIDOS\tCHIPS");
                salida.AppendLine(usu.Id + "\t" + usu.NombreUsuario + "\t" + usu.NombreReal + "\t" + usu.NombreDeFoto + "\t" + usu.Seguidores + "\t\t" + usu.Seguidos + "\t" + usu.CantidadChips);
                GestorImpresion.Imprimir(salida.ToString());
                // La impresion del resultado puede tener impresiones en el medio
                if (usu.CantidadChips > 0)
                {
                    MostrarChips(usu.Chips);
                }
            }
        }

        private static void MostrarUsuarios(ResultadoOperacion resultado)
        {
            try
            {
                StringBuilder salida = new StringBuilder();
                List<UsuarioDTO> usuarios = (List<UsuarioDTO>)resultado.Entidad;
                salida.AppendLine("\nUSUARIOS DEL SISTEMA:\n");
                salida.AppendLine("ID\tNOMBRE USUARIO\t\tNOMBRE REAL\t\tSEGUIDORES\tSEGUIDOS");
                foreach (UsuarioDTO usu in usuarios)
                {
                    salida.AppendLine(usu.Id + "\t" + usu.NombreUsuario + "\t\t" + usu.NombreReal + "\t\t" + usu.Seguidores + "\t\t" + usu.Seguidos);
                }
                GestorImpresion.Imprimir(salida.ToString());
            }
            catch (Exception ex)
            {
                GestorImpresion.Imprimir(ex.Message);
            }

        }
    }
}
