using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Excepciones
{
    internal class MensajesExcepciones
    {
        public const string ArchivoNulo = "El archivo indicado es nulo.";
        public const string ArgumentoNoValido = "El argumento no es válido, verifique que la ruta indicada sea la correcta.";
        public const string NoAutorizado = "No posee permisos para acceder a la ubicación indicada.";
        public const string RutaDemasiadoLarga = "La ruta indicada es demasiado larga.";
        public const string FuncionalidadNoSoportada = "La funcionalidad que intenta acceder aún no ha sido desarrollada."; 
        public const string ArchivoNoEncontrado = "No fue posible encontrar el archivo en la ruta indicada."; 
        public const string DirectorioNoEncontrado = "No fue posible encontrar la carpeta en la ruta indicada.";
        public const string ErrorEnCarga = "Hubo un error al cargar el archivo, verifique la ruta e intente nuevamente.";
        public const string GenericaIO = "Hubo un error en el acceso a un archivo, verifique la información de la ruta e intente nuevamente.";
        public const string ErrorLeyendoArchivo = "Error generado en la lectura del archivo durante su envio, se cerrará la conexión.";
        public const string Generica = "Hubo un en el acceso a un archivo, verifique la información de la ruta e intente nuevamente."; // VER MENSAJE

    }
}
