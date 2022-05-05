using Servicios.DTOs;

namespace Servicios.Interfaces
{
    public interface IGestorEnvioArchivos
    {
        public void EnviarArchvivo(string ruta, ISocketHelper socketHelper);
        public void RecibirArchvivo(MetadataFotoDTO dtoFoto, ISocketHelper socketHelper);
        public string ObtenerMetadataFoto(string ruta);
    }
}
