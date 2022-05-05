using Cliente.Impresion;
using Cliente.LogicaCliente;
using System.Text;

namespace Cliente.Menus
{
    internal class MenuUsuarioDesconectado
    {
        private bool _salir;
        static readonly EstadoCliente estadoCliente = EstadoCliente.ObtenerInstancia();

        private enum OpcionMenu
        {
            Alta_de_usuario,
            Login,
            Conectarse_al_servidor,
            Desconectarse_del_servidor,
            Finalizar
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
            _salir = false;
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
                        case OpcionMenu.Alta_de_usuario:
                            GestorUsuarioDesconectado.AltaDeUsuario(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Login:
                            GestorUsuarioDesconectado.LoginUsuario(estadoCliente._conexion);
                            _salir = true;
                            EsperarRespuestaLogin();
                            break;
                        case OpcionMenu.Conectarse_al_servidor:
                            GestorUsuarioDesconectado.ConectarseAlServidor(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Desconectarse_del_servidor:
                            GestorUsuarioDesconectado.Desconectarse(estadoCliente._conexion);
                            break;
                        case OpcionMenu.Finalizar:
                            GestorUsuarioDesconectado.Desconectarse(estadoCliente._conexion);
                            _salir = true;
                            break;
                        default:
                            GestorImpresion.Imprimir("Menú Deslogueado: Seleccione una opcion válida.");
                            break;
                    }
                    GestorImpresion.Imprimir("\n*****************************************\n");
                }
                catch (ArgumentException ex)
                {
                    GestorImpresion.Imprimir("Menú Deslogueado: Argumento no válido.");
                } catch (Exception ex)
                {
                    GestorImpresion.Imprimir("Menú Deslogueado: Error inesperado, intente nuevamente.");
                }
            }
        }
        private void EsperarRespuestaLogin()
        {
            int contador = 0;
            while(contador < 1000 && estadoCliente._usuario == null)
            {
                GestorTransmisiones.GestionarTodasLasTransmisiones();
                Thread.Sleep(50);
                contador+=50;
            }
            if(estadoCliente._usuario == null)
            {
                GestorImpresion.Imprimir("El login no resultó satisfactorio.\n");
                Menu();
            }
        }
    }
}
