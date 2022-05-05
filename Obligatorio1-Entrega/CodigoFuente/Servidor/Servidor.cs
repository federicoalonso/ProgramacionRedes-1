using Servicios;
using Servicios.Interfaces;
using Servidor.LogicaNegocio;
using Servidor.Pantalla;
using Servidor.Repositorio;
using System.Net;
using System.Net.Sockets;

namespace Servidor
{
    internal class Servidor
    {
        static readonly IGestorConfiguracion _gestorConfiguracion = FabricaInterfaces.FabricaGestorConfiguracion();

        int serverPort = int.Parse(_gestorConfiguracion.LeerConfiguracion(ConfiguracionServidor.PuertoServidor));
        string ipAddress = _gestorConfiguracion.LeerConfiguracion(ConfiguracionServidor.IPServidor);
        int backLog = int.Parse(_gestorConfiguracion.LeerConfiguracion(ConfiguracionServidor.BackLog));
        bool servidorEncendido = false;

        private static Servidor Instancia = null;
        private static readonly object CandadoInstancia = new Object();

        IPEndPoint serverEndpoint;
        Socket serverSocket;

        Thread hiloServidor;

        private Servidor()
        {
            serverEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), serverPort);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        internal static Servidor ObtenerInstancia()
        {
            if (Servidor.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (Servidor.Instancia == null)
                    {
                        Servidor.Instancia = new Servidor();
                    }
                }
            }
            return Servidor.Instancia;
        }

        internal void EncenderServicio()
        {
            if (!servidorEncendido)
            {
                GestorImpresion.Imprimir("Iniciando servidor");

                try
                {
                    serverSocket.Bind(serverEndpoint);
                    serverSocket.Listen(backLog);
                    GestorImpresion.Imprimir($"Servidor escuchando en el puerto {serverPort}");
                    GestorLog.EscribirLog($"Servidor escuchando en el puerto {serverPort}");
                    servidorEncendido = true;

                    new Thread(() => EscucharClientes()).Start();
                }
                catch (SocketException ex)
                {
                    GestorLog.EscribirLog($"Socket error: {ex.SocketErrorCode} / {ex.ErrorCode} msg: {ex.Message}");
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close();
                    GestorImpresion.Imprimir("Servicio Apagado");
                }
                catch (Exception ex)
                {
                    GestorLog.EscribirLog(ex.ToString());
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close();
                    GestorImpresion.Imprimir("Servicio Apagado");
                }
            }
            else
            {
                GestorImpresion.Imprimir($"El servicio ya se encuentra a la escucha en el puerto {serverPort}");
            }
        }
        internal void ApagarServicio()
        {
            GestorImpresion.Imprimir("Apagando servidor");

            try
            {
                RepositorioConexion repo = RepositorioConexion.ObtenerInstancia();
                repo.CerrarConexiones();
                serverSocket.Close();
            }
            catch (SocketException ex)
            {
                GestorLog.EscribirLog($"Socket error: {ex.SocketErrorCode} / {ex.ErrorCode} msg: {ex.Message}");
            }
            catch (Exception ex)
            {
                GestorLog.EscribirLog(ex.ToString());
            }
            finally
            {
                serverSocket.Close();
                GestorImpresion.Imprimir("Servicio Apagado");
            }
        }
        private void EscucharClientes()
        {
            while (servidorEncendido)
            {
                try
                {
                    var clientSocket = serverSocket.Accept();
                    GestorLog.EscribirLog("Cliente conectado.");

                    hiloServidor = new Thread(() => GestorConexiones.GestionarCliente(clientSocket));
                    hiloServidor.Start();
                }
                catch (SocketException ex)
                {
                    GestorLog.EscribirLog($"Socket error: {ex.SocketErrorCode} / {ex.ErrorCode} msg: {ex.Message}");
                }
                catch (Exception ex)
                {
                    GestorLog.EscribirLog(ex.ToString());

                }
            }
        }
    }
}
