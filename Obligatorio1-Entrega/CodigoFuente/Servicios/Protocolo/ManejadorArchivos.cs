using Servicios.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Protocolo
{
    internal class ManejadorArchivos
    {
        public bool ExisteArchivo(string ruta)
        {
            return File.Exists(ruta);
        }

        public string ObtenerNombreArchivo(string ruta)
        {
            try
            {
                return ObtenerInformacionDeArchivo(ruta).Name;
            }
            catch (FileNotFoundException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.ArchivoNoEncontrado, excepcion);
            }
        }

        private static FileInfo ObtenerInformacionDeArchivo(string ruta)
        {
            try
            {
                return new FileInfo(ruta);
            }
            catch (ArgumentNullException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.ArchivoNulo, excepcion);
            }
            catch (ArgumentException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.ArgumentoNoValido, excepcion);
            }
            catch (UnauthorizedAccessException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.NoAutorizado, excepcion);
            }
            catch (PathTooLongException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.RutaDemasiadoLarga, excepcion);
            }
            catch (NotSupportedException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.FuncionalidadNoSoportada, excepcion);
            }
        }

        public long ObtenerDimensionArchivo(string ruta)
        {
            try
            {
                return ObtenerInformacionDeArchivo(ruta).Length;
            }
            catch (FileNotFoundException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.ArchivoNoEncontrado, excepcion);
            }
            catch (IOException excepcion)
            {
                throw new ExcepcionEnAccesoAArchivo(MensajesExcepciones.GenericaIO, excepcion);
            }
        }  
    }
}
