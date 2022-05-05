using Cliente.Impresion;
using Cliente.LogicaCliente;
using System.Text;

namespace Cliente.Menus
{
    /*
     * Responsabilid: 
     * - Mostrar el menu de usuario
     */
    internal class MenuUsuarioLogueado
    {
        static readonly EstadoCliente estadoCliente = EstadoCliente.ObtenerInstancia();

        private enum OpcionMenu
        {
            Ver_usuarios,
            Buscar_usuarios_por_parametro,
            Seguir_usuarios,
            Crear_chip,
            Ver_perfil,
            Responder_chip,
            Asociar_foto_al_perfil,
            Buscar_chips_por_palabra,
            Recargar,
            Logout 
        }

        private string ConstruirMenu()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("SELECCIONE UNA OPCION:").AppendLine();
            foreach (int i in Enum.GetValues(typeof(OpcionMenu)))
            {
                string nombre = Enum.GetName(typeof(OpcionMenu), i);
                str.AppendLine($"{(int)i} - {nombre.Replace("_", " ")}");
            }
            str.AppendLine();
            return str.ToString();
        }

        internal void Menu()
        {
            bool _salir = false;
            while (!_salir)
            {
                try
                {
                    string menu = ConstruirMenu();

                    GestorTransmisiones.GestionarTodasLasTransmisiones();

                    GestorImpresion.Imprimir(menu);
                    OpcionMenu num = (OpcionMenu)Enum.Parse(typeof(OpcionMenu), Console.ReadLine());
                    switch (num)
                    {
                        case OpcionMenu.Ver_usuarios:
                            GestorUsuarioLogueado.VerUsuariosDelSistema(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Buscar_usuarios_por_parametro:
                            GestorUsuarioLogueado.BuscarUsuario(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Seguir_usuarios:
                            GestorUsuarioLogueado.SeguirUsuario(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Crear_chip:
                            GestorUsuarioLogueado.CrearChip(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Ver_perfil:
                            GestorUsuarioLogueado.VerPerfilDeUsuario(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Responder_chip:
                            GestorUsuarioLogueado.ResponderChip(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Asociar_foto_al_perfil:
                            GestorUsuarioLogueado.AsociarFotoAlPerfil(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Buscar_chips_por_palabra:
                            GestorUsuarioLogueado.BuscarChipsPorPalabra(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Recargar:
                            break;
                        case OpcionMenu.Logout:
                            _salir = true;
                            GestorUsuarioLogueado.Logout(estadoCliente._conexion);
                            EsperarRespuestaLogout();
                            break;
                        default:
                            GestorImpresion.Imprimir("Menú Logueado: Seleccione una opcion valida.");
                            break;
                    }
                    GestorImpresion.Imprimir("\n\n*****************************************\n\n");
                }
                catch (ArgumentException ex)
                {
                    GestorImpresion.Imprimir("Menú Logueado: Argumento no válido.");
                }
                catch (Exception)
                {
                    GestorImpresion.Imprimir("Menú Logueado: Error inesperado, intente nuevamente.");
                }
            }
        }
        private void EsperarRespuestaLogout()
        {
            int contador = 0;
            while (contador < 1000 && estadoCliente._usuario != null)
            {
                GestorTransmisiones.GestionarTodasLasTransmisiones();
                Thread.Sleep(50);
                contador += 50;
            }
            if (estadoCliente._usuario != null)
            {
                GestorImpresion.Imprimir("El logout no resultó satisfactorio.\n");
                Menu();
            }
        }
    }
}
