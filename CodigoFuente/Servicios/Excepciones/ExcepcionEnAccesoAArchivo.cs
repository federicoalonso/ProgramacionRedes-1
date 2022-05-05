using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Excepciones
{
    [Serializable]
    public class ExcepcionEnAccesoAArchivo : Exception
    {
        public readonly string mensaje;
        public readonly Exception excepcionInterna;
        public override string Message => mensaje;

        public ExcepcionEnAccesoAArchivo(string unMensaje, Exception exception)
        {
            mensaje = unMensaje;
            excepcionInterna = exception;
        }
    }
}