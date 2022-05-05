using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Servicios.Excepciones;

namespace Servicios.Protocolo
{
    public class ManejadorFileStram
    {
        private readonly ManejadorArchivos _manejadorArchivos;

        public ManejadorFileStram()
        {
            _manejadorArchivos = new ManejadorArchivos();   
        }

        public byte[] Leer(string ruta, long restante, int largo)
        {
            try
            {
                var datos = new byte[largo];
                using var fileStream = CrearFileStream(ruta, FileMode.Open);
                fileStream.Position = restante;
                var bytesLeidos = 0;
                while (bytesLeidos < largo)
                {
                    var leido = fileStream.Read(datos, bytesLeidos, largo - bytesLeidos);
                    if (leido <= 0)
                    {
                        throw new ExcepcionEnLecturaDeArchivoDuranteEnvio(MensajesExcepciones.ErrorLeyendoArchivo);
                    }
                    bytesLeidos += leido;
                }
                return datos;
            }
            catch (ArgumentException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.ArgumentoNoValido, excepcion);
            }
            catch (NotSupportedException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.FuncionalidadNoSoportada, excepcion);
            }
            catch (Exception excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.GenericaIO, excepcion);
            }
        }

        public void Escribir(string nombreArchivo, byte[] datos)
        {
            var modoArchivo = _manejadorArchivos.ExisteArchivo(nombreArchivo) ? FileMode.Append : FileMode.Create;
            //La creacion del archivo es en la ruta donde está el proyecto, se puede modificar pasando una nueva ruta en el constructor
            using var fileStream = CrearFileStream(nombreArchivo, modoArchivo);
            fileStream.Write(datos, 0, datos.Length);
        }

        private static FileStream CrearFileStream(string ruta, FileMode modo)
        {
            try
            {
                return new FileStream(ruta, modo);
            }
            catch (FileNotFoundException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.ArchivoNoEncontrado, excepcion);
            }
            catch (UnauthorizedAccessException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.NoAutorizado, excepcion);
            }
            catch (DirectoryNotFoundException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.DirectorioNoEncontrado, excepcion);
            }
            catch (PathTooLongException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.RutaDemasiadoLarga, excepcion);
            }
            catch (FileLoadException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.ErrorEnCarga, excepcion);
            }
            catch (IOException excepcion)
            {
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.GenericaIO, excepcion);
            }         
        }

        public static void VerificarAccesibilidadDeArchivo(string ruta)
        {
            FileStream esAccesible = null;
            try
            {
                esAccesible = new FileStream(ruta, FileMode.Open);
                esAccesible.Close();
            } 
            catch (UnauthorizedAccessException excepcion)
            {
                esAccesible.Close();
                throw new ExcepcionEnManejadorFileStream(MensajesExcepciones.NoAutorizado, excepcion);
            }
        }
    }
}
