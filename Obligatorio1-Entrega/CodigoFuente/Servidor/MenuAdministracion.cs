using Servicios;
using Servicios.DTOs;
using Servidor.Dominio;
using Servidor.LogicaNegocio;
using Servidor.Pantalla;
using Servidor.Servicios;
using System.Text;

namespace Servidor
{
    internal class MenuAdministracion
    {
        //internal ConfiguracionServidor _configuracion = new ConfiguracionServidor();
        GestorChip _gestorChip = new GestorChip();
        GestorUsuario _gestorUsuario = new GestorUsuario();
        GestorConexiones _gestorConexiones = new GestorConexiones();
        FabricaDatosPrueba _fabricaDatosPrueba = new FabricaDatosPrueba();
        Servidor _servidor = Servidor.ObtenerInstancia();
        private static readonly object CandadoConsola = new Object();

        private enum OpcionMenu
        {
            Cargar_datos_de_prueba,
            Ver_usuarios,
            Bloquear_usuarios,
            Autorizar_usuarios,
            Encender_servicio,
            Buscar_chips_por_palabra,
            Lista_de_usuarios_mas_seguidos,
            Lista_de_usuario_mas_activos,
            Finalizar
        }

        internal void AccesoAdministrador()
        {
            bool _salir = false;

            while (!_salir)
            {
                try
                {
                    StringBuilder salida = new StringBuilder();
                    salida.AppendLine("\n\nSELECCIONE UNA OPCION: \n");
                    foreach (int i in Enum.GetValues(typeof(OpcionMenu)))
                    {
                        string nombre = Enum.GetName(typeof(OpcionMenu), i);
                        salida.AppendLine($"{(int)i} - {nombre.Replace("_", " ")}");
                    }
                    salida.AppendLine("\n");
                    GestorImpresion.Imprimir(salida.ToString());

                    OpcionMenu num = (OpcionMenu)Enum.Parse(typeof(OpcionMenu), Console.ReadLine());

                    switch (num)
                    {
                        case OpcionMenu.Cargar_datos_de_prueba:
                            IngresarDatosPrueba();
                            break;
                        case OpcionMenu.Ver_usuarios:
                            VerUsuariosDelSistema();
                            break;
                        case OpcionMenu.Bloquear_usuarios:
                            BloquearUsuario();
                            break;
                        case OpcionMenu.Autorizar_usuarios:
                            AutorizarUsuario();
                            break;
                        case OpcionMenu.Encender_servicio:
                            EncenderServicio();
                            break;
                        case OpcionMenu.Buscar_chips_por_palabra:
                            BuscarChipsPorPalabras();
                            break;
                        case OpcionMenu.Lista_de_usuarios_mas_seguidos:
                            VerMasSeguidos();
                            break;
                        case OpcionMenu.Lista_de_usuario_mas_activos:
                            VerUsuariosMasActivos();
                            break;
                        case OpcionMenu.Finalizar:
                            FinalizarConexion();
                            _salir = true;
                            break;
                        default:
                            Console.WriteLine("Seleccione una opcion válida.");
                            break;
                    }
                    GestorImpresion.Imprimir("\n\n*****************************************\n\n");
                }
                catch (ArgumentException ex)
                {
                    GestorImpresion.Imprimir("Argumento no válido.");
                }
                catch (Exception ex)
                {
                    GestorLog.EscribirLog(ex.Message);
                    GestorImpresion.Imprimir("Error en el menú administración.");
                }
            }
        }

        internal void VerUsuariosMasActivos()
        {
            double periodo = -1;
            PantallaServidor pantallaBloqueada = GestorImpresion.BloquearPantalla();

            pantallaBloqueada.Imprimir("Ingrese el período de tiempo en horas deseado para la búsqueda de actividad:");
            string valor;
            bool valorValido = false;
            while (!valorValido)
            {
                try
                {
                    valor = Console.ReadLine();
                    periodo = double.Parse(valor, System.Globalization.CultureInfo.InvariantCulture);
                    if (periodo < 0)
                    {
                        pantallaBloqueada.Imprimir("Ingrese un valor mayor o igual a 0");
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
                catch (OverflowException desbordeTipo)
                {
                    pantallaBloqueada.Imprimir($"Ingrese un valor númerico más pequeño");
                }
            }
            
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("USUARIOS MÁS ACTIVOS:\n");
            List<Chip> chipsDurantePeriodo = _gestorChip.ChipsPorPeriodoDeTiempo(periodo);

            List<Usuario> usuarios = _gestorUsuario.ObtenerUsuariosMasActivos(chipsDurantePeriodo);
            if (usuarios.Count > 0)
            {
                salida.AppendLine("ID\tNOMBRE USUARIO\tNOMBRE REAL\tSEGUIDORES\tSEGUIDOS\tBLOQUEADO\tCHIPS");
                foreach (Usuario usu in usuarios)
                {
                    salida.AppendLine(usu.ToString() + " - " + _gestorChip.CantidadChipsPorUsuario(usu.Id));
                }
            }
            else
            {
                salida.AppendLine("\n\nNo hay usuarios con actividad reportable\n\n");
            }

            pantallaBloqueada.Imprimir(salida.ToString());
        }

        private void BuscarChipsPorPalabras()
        {
            string cadenaBuscada;
            GestorImpresion.Imprimir("Escriba la cadena a buscar entre los chips");
            cadenaBuscada = Console.ReadLine();
            BusquedaChipsDTO busquedaChipDTO = new BusquedaChipsDTO()
            {
                CadenaBuscada = cadenaBuscada
            };
            ResultadoOperacion resultado = _gestorChip.BuscarChipsPorPalabra(busquedaChipDTO);
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("RESULTADO BUSQUEDA DE CHIPS POR PALABRA: \n");
            if (resultado.Entidad == null)
                salida.AppendLine(resultado.Mensaje);
            else
            {
                int contadorChips = 1;
                List<Chip> chips = (List<Chip>)resultado.Entidad;
                foreach (Chip chipDTO in chips)
                {
                    salida.AppendLine($"Chip número {contadorChips}:");
                    salida.AppendLine($"\tPublicado por: {chipDTO.UsuarioId}");
                    salida.AppendLine($"\tChip Id: {chipDTO.Id}");
                    if (chipDTO.ChipPadreId != -1)
                    {
                        salida.AppendLine($"\tEn respuesta a chip: {chipDTO.ChipPadreId}");
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
            }
            GestorImpresion.Imprimir(salida.ToString());
        }

        private void EncenderServicio()
        {
            _servidor.EncenderServicio();
        }

        internal void FinalizarConexion()
        {
            _servidor.ApagarServicio();
            
        }

        private void VerMasSeguidos()
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("\n\nUSUARIOS MAS SEGUIDOS DEL SISTEMA:\n");
            // falta cantidad de chips
            ResultadoOperacion resultadoMasSeguidos = _gestorUsuario.VerUsuariosMasSeguidos();
            List<Usuario> usuariosSeguidos = (List<Usuario>)resultadoMasSeguidos.Entidad;
            salida.AppendLine("ID\tNOMBRE USUARIO\tSEGUIDORES");
            for (int i = 0; i < 5; i++)
            {
                Usuario usu = usuariosSeguidos[i];
                salida.AppendLine(usu.Id + "\t" + usu.NombreUsuario + "\t" + usu.Seguidores.Count);
            }
            GestorImpresion.Imprimir(salida.ToString());
        }

        private void AutorizarUsuario()
        {
            PantallaServidor pantallaBloqueada = GestorImpresion.BloquearPantalla();
            pantallaBloqueada.Imprimir("Ingrese el ID del usuario a autorizar acceso:");
            int idAut = Convert.ToInt32(Console.ReadLine());
            ResultadoOperacion resultadoAutorizar = _gestorUsuario.AutorizarUsuario(idAut);
            pantallaBloqueada.Imprimir(resultadoAutorizar.Mensaje);
        }

        private void BloquearUsuario()
        {
            PantallaServidor pantallaBloqueada = GestorImpresion.BloquearPantalla();
            pantallaBloqueada.Imprimir("Ingrese el ID del usuario a bloquear:");
            int id = Convert.ToInt32(Console.ReadLine());
            ResultadoOperacion resultadoBloquear = _gestorUsuario.BloquearUsuario(id);
            _gestorConexiones.DesconectarUsuario(id);
            pantallaBloqueada.Imprimir(resultadoBloquear.Mensaje);
        }

        private void VerUsuariosDelSistema()
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("USUARIOS DEL SISTEMA:\n");
            ResultadoOperacion resultado1 = _gestorUsuario.VerUsuarios();
            List<Usuario> usuarios = (List<Usuario>)resultado1.Entidad;
            salida.AppendLine("ID\t\tNOMBRE USUARIO\tNOMBRE REAL\tSEGUIDORES\tSEGUIDOS\tBLOQUEADO\tCHIPS");
            foreach (Usuario usu in usuarios)
            {
                salida.AppendLine(usu.ToString() + " - " + _gestorChip.CantidadChipsPorUsuario(usu.Id));
            }
            GestorImpresion.Imprimir(salida.ToString());
        }

        private void IngresarDatosPrueba()
        {
            PantallaServidor pantallaBloqueada = GestorImpresion.BloquearPantalla();
            _fabricaDatosPrueba.CargarDatosDePrueba();
            pantallaBloqueada.Imprimir("Datos cargados con exito");
        }
    }
}
