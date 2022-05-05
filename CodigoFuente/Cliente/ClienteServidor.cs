using Cliente.Impresion;
using Cliente.LogicaCliente;
using Servicios;
using Servicios.Conexiones;
using Servicios.Interfaces;
using Servicios.Protocolo;
using System.Net;
using System.Net.Sockets;

namespace Cliente
{
    /*
     * Responsabilidades:
     * - Conectarse al servidor
     * - Enviarle mensajes al servidor
     * - Recibir mensajes del servidor
     * - Desconectarse del servidor
     * - Enviar Archivos al servidor
     */
    internal class ClienteServidor
    {
        private static readonly IGestorConfiguracion _gestorConfiguracion = FabricaInterfaces.FabricaGestorConfiguracion();

        private static int serverPort = int.Parse(_gestorConfiguracion.LeerConfiguracion(ConfiguracionCliente.PuertoServidor));
        private string serverHost = _gestorConfiguracion.LeerConfiguracion(ConfiguracionCliente.IPServidor);
        private string ipLocal = _gestorConfiguracion.LeerConfiguracion(ConfiguracionCliente.IPLocal);

        private bool conectadoAlServidor = false;

        private Socket clientSocket;
        private IConexion conexion;
        
        internal void ConectarseAlServidor()
        {
            if (!conectadoAlServidor)
            {
                try
                {
                    var serverEndpoint = new IPEndPoint(IPAddress.Parse(serverHost), serverPort);
                    var clientEndpoint = new IPEndPoint(IPAddress.Parse(ipLocal), 0);
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocket.Bind(clientEndpoint);
                    clientSocket.Connect(serverEndpoint);
                    conexion = FabricaInterfaces.FabricaConexion();
                    conexion.SocketHelp = FabricaInterfaces.FabricaSocketHelper(clientSocket);
                    GestorImpresion.Imprimir("Se conecta el cliente al servidor");
                    GestorLog.EscribirLog("Cliente conectado al servidor.");
                    conectadoAlServidor = true;
                    new Thread(() => EscucharMensajes()).Start();
                }
                catch (Exception ex)
                {
                    GestorImpresion.Imprimir("Error: No se pudo conectar al servidor.");
                    GestorLog.EscribirLog(ex.Message);
                }
            }
            else
            {
                GestorImpresion.Imprimir("ERROR: Ya se encuentra conectado.");
            }
        }

        internal void EnviarTransmision(Transmision t)
        {
            try
            {
                conexion.EnviarTransmision(t);
            }
            catch (NullReferenceException ex)
            {
                GestorImpresion.Imprimir("\nERROR: Debe conectarse al servidor para enviar transmisiones.\n");
                conectadoAlServidor = false;
            }
            catch (Exception ex)
            {
                GestorImpresion.Imprimir(ex.Message);
                GestorLog.EscribirLog(ex.Message);
                conectadoAlServidor = false;
            }
        }

        internal void Desconectarse()
        {
            if (conectadoAlServidor)
            {
                try
                {
                    if (clientSocket != null)
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                    }
                }catch(SocketException ex)
                {
                    GestorLog.EscribirLog($"Error en Socket, código: {ex.SocketErrorCode} / {ex.ErrorCode} mensaje: {ex.Message}");
                    conectadoAlServidor = false;
                }
                catch(Exception e)
                {
                    GestorLog.EscribirLog(e.Message);
                    conectadoAlServidor = false;
                }
                finally
                {
                    GestorImpresion.Imprimir("Se ha desconectado del servidor.");
                    conectadoAlServidor = false;
                    clientSocket.Close();
                }
            }
            else
            {
                GestorImpresion.Imprimir("ERROR: Usted no se encuentra conectado al servidor.");
            }
        }

        internal void EscucharMensajes()
        {
            try
            {
                while (conectadoAlServidor)
                {
                    Transmision t = conexion.EscucharMensajes(HeaderConstantes.Response.Length + HeaderConstantes.ComandosLength + HeaderConstantes.DataLength);
                    GestorLog.EscribirLog($"Servidor dice: {t.BodyT} + {Thread.CurrentThread.Name}");
                    GestorTransmisiones.GestionarTransmision(t);
                }
            }
            catch (SocketException ex)
            {
                GestorLog.EscribirLog($"Error en Socket, código: {ex.SocketErrorCode} / {ex.ErrorCode} mensaje: {ex.Message}");
                conectadoAlServidor = false;
            }
            catch (Exception ex)
            {
                GestorLog.EscribirLog(ex.Message);
                conectadoAlServidor = false;
            }
            finally
            {
                conectadoAlServidor = false;
                conexion = null;
                GestorImpresion.Imprimir("Se ha desconectado del servidor.");
                clientSocket.Close();
            }
        }

        internal void EnviarArchivo(string? ruta)
        {
            try
            {
                conexion.EnviarArchivo(ruta);
            }
            catch (NullReferenceException)
            {
                GestorImpresion.Imprimir("\nERROR: Debe conectarse al servidor para enviar transmisiones.\n");
            }
        }
    }
}
