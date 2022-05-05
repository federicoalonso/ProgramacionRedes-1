using Servicios.Interfaces;
using System.Configuration;

namespace Servicios
{
    public class GestorConfiguracion : IGestorConfiguracion
    {
        public string LeerConfiguracion(string clave)
        {
            try
            {
                var appConfiguracion = ConfigurationManager.AppSettings;
                return appConfiguracion[clave] ?? string.Empty;
            }catch (ConfigurationException ex)
            {
                GestorLog.EscribirLog(ex.Message);
                return string.Empty;
            }catch(Exception ex)
            {
                GestorLog.EscribirLog(ex.Message);
                return string.Empty;
            }
        }
    }
}
