using Servicios.Protocolo;

namespace Servicios
{
    public class Validaciones
    {
        public static ResultadoOperacion ValidarLargoTexto(string texto, int largoMax, int largoMin, string campo)
        {
            texto = texto.Trim();
            ResultadoOperacion validacion = new ResultadoOperacion();
            if (texto.Length > largoMax || texto.Length < largoMin)
            {
                validacion.Mensaje = "El largo del campo " + campo + " debe ser de entre " + largoMin.ToString() + " y " + largoMax.ToString() + " caracteres.";
                validacion.Codigo = CodigosConstantes.ArgumentoNoValido;
            }
            else
            {
                validacion.Codigo = CodigosConstantes.TransmisionOK;
            }
            return validacion;
        }

        public static ResultadoOperacion ValidarPassword(string texto)
        {
            bool noValido = texto.Contains(" ");
            ResultadoOperacion validacion = new ResultadoOperacion();
            if (noValido)
            {
                validacion.Mensaje = "No puede contener espacios en blanco en la contraseña";
                validacion.Codigo = CodigosConstantes.ArgumentoNoValido;
            }
            else if (texto.Length > 25 || texto.Length < 8)
            {
                validacion.Mensaje = "El largo de la contraseña debe ser de entre 8 y 25 caracteres.";
                validacion.Codigo = CodigosConstantes.ArgumentoNoValido;
            }
            else
            {
                validacion.Codigo = CodigosConstantes.TransmisionOK;
            }
            return validacion;
        }
    }
}
