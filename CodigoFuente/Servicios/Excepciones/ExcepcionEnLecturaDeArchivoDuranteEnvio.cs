using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Excepciones
{
    public class ExcepcionEnLecturaDeArchivoDuranteEnvio : Exception
    {
        public readonly string mensaje;
        public readonly Exception excepcionInterna;
        public override string Message => mensaje;

        public ExcepcionEnLecturaDeArchivoDuranteEnvio(string unMensaje, Exception exception)
        {
            mensaje = unMensaje;
            excepcionInterna = exception;
        }

        public ExcepcionEnLecturaDeArchivoDuranteEnvio(string? mensaje) : base(mensaje)
        {
        }
    }
}
