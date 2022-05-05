using Servicios.DTOs;
using Servicios.Interfaces;

namespace Servicios.Protocolo
{
    public class GestorEnvioArchivos : IGestorEnvioArchivos
    {
        public void EnviarArchvivo(string ruta, ISocketHelper socketHelper)
        {
            ManejadorArchivos _manejadorArchivos = new ManejadorArchivos();
            ManejadorFileStram _manejadorFileStream = new ManejadorFileStram();
            long tamanioArchivo = _manejadorArchivos.ObtenerDimensionArchivo(ruta);
            long fragmentosDeArchivo = HeaderConstantes.CalcularParticionesDeArchivo(tamanioArchivo);
            long enviado = 0;
            long fragmentoActual = 1;

            while(tamanioArchivo > enviado)
            {
                byte[] datos;
                if (fragmentoActual == fragmentosDeArchivo)
                {
                    var tamanioUltimoFragmento = (int)(tamanioArchivo - enviado);
                    // Lanza excepcionEnManejadorFileStream la excepcion ExcepcionEnLecturaDeArchivoDuranteEnvio se maneja desde afuera y el catch cierra la conexion
                    datos = _manejadorFileStream.Leer(ruta, enviado, tamanioUltimoFragmento);
                    enviado += tamanioUltimoFragmento;
                } 
                else
                {
                    // Lanza excepcionEnManejadorFileStream la excepcion ExcepcionEnLecturaDeArchivoDuranteEnvio se maneja desde afuera y el catch cierra la conexion
                    datos = _manejadorFileStream.Leer(ruta, enviado, HeaderConstantes.LargoMaximoDatos);
                    enviado += HeaderConstantes.LargoMaximoDatos;
                }
                socketHelper.Send(datos);
                fragmentoActual++;
            }
        }

        public void RecibirArchvivo(MetadataFotoDTO dtoFoto,  ISocketHelper socketHelper)
        {
            ManejadorFileStram _manejadorFileStream = new ManejadorFileStram();
            // Try y catch si ya existe el nombre del archivo para no sobreescribir?
            long tamanioArchivo = dtoFoto.TamanioArchivo;
            string nombreArchivo = dtoFoto.Nombre;
            long fragmentosDeArchivo = HeaderConstantes.CalcularParticionesDeArchivo(tamanioArchivo);
            long enviado = 0;
            long fragmentoActual = 1;

            while (tamanioArchivo > enviado)
            {
                byte[] datos;
                if (fragmentoActual == fragmentosDeArchivo)
                {
                    var tamanioUltimoFragmento = (int)(tamanioArchivo - enviado);
                    datos = socketHelper.Receive(tamanioUltimoFragmento);
                    enviado += tamanioUltimoFragmento;
                }
                else
                {
                    datos = socketHelper.Receive(HeaderConstantes.LargoMaximoDatos);
                    enviado += HeaderConstantes.LargoMaximoDatos;
                }
                // Lanza excepcionEnManejadorFileStream la excepcion ExcepcionEnLecturaDeArchivoDuranteEnvio se maneja desde afuera y el catch cierra la conexion
                _manejadorFileStream.Escribir(nombreArchivo, datos);
                fragmentoActual++;
            }
        }

        public string ObtenerMetadataFoto(string ruta)
        {
            string retorno;
            ManejadorArchivos _manejadorArchivos = new ManejadorArchivos();
                string nombreArchivo = _manejadorArchivos.ObtenerNombreArchivo(ruta);
                long tamanioArchivo = _manejadorArchivos.ObtenerDimensionArchivo(ruta);
                MetadataFotoDTO metadataFotoDTO = new MetadataFotoDTO()
                {
                    Nombre = nombreArchivo,
                    LargoNombre = nombreArchivo.Length,
                    TamanioArchivo = tamanioArchivo,
                };
                retorno = metadataFotoDTO.ToString();
            return retorno;        
        }
    }
}
