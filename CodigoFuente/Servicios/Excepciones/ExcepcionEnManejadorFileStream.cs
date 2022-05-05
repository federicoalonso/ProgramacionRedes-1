using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Excepciones
{
    [Serializable]
    public class ExcepcionEnManejadorFileStream : Exception
    {
        public readonly string mensaje;
        public readonly Exception excepcionInterna;
        public override string Message => mensaje;

        public ExcepcionEnManejadorFileStream(string unMensaje, Exception exception)
        {
            mensaje = unMensaje;
            excepcionInterna = exception;
        }

        public ExcepcionEnManejadorFileStream(string? mensaje) : base(mensaje)
        {
        }
    }
}