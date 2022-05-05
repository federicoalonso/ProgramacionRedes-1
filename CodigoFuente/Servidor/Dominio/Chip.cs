using Servicios.DTOs;

namespace Servidor.Dominio
{
    internal class Chip
    {
        internal int Id { get; set; }
        internal string Cuerpo { get; set; }
        internal int UsuarioId { get; set; }
        internal int ChipPadreId { get; set; }
        internal int CantidadDeFotos { get; set; }
        internal DateTime Fecha { get; set; }
        internal List<MetadataFotoDTO> metadataFotos { get; set; }


        internal Chip()
        {
            this.metadataFotos = new List<MetadataFotoDTO>();
            this.Fecha = DateTime.Now;
        }

        public override string ToString()
        {
            return "Chip Id " + this.Id + " -\t" + "Contenido: " + this.Cuerpo
                + " -\t" + "Publicado por: " + this.UsuarioId + " -\t" + "En respuesta a: " + this.ChipPadreId
                + " -\t" + "Cantidad de Fotos: " + this.CantidadDeFotos;
        }
    }

}
