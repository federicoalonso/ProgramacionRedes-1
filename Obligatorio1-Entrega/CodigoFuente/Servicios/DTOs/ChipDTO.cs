using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.DTOs
{
    public class ChipDTO
    {
        public int Id { get; set; }
        public string Cuerpo { get; set; }
        public int UsuarioId { get; set; }
        public int ChipPadreId { get; set; }
        public int CantidadDeFotos { get; set; }
        public List<MetadataFotoDTO> metadataFotos { get; set; }
        public List<string> rutasFotos { get; set; }

        public ChipDTO()
        {
            this.rutasFotos = new List<string>();
            this.metadataFotos = new List<MetadataFotoDTO>();
        }

        public override string ToString()
        {
            string objString = "";
            objString += "|Cuerpo:" + Cuerpo;
            objString += "|UsuarioId:" + UsuarioId;
            objString += "|ChipPadreId:" + ChipPadreId;
            objString += "|CantidadDeFotos:" + CantidadDeFotos;
            if (CantidadDeFotos> 0)
            {
                objString += "|MetadataFotos:";
                for (int indice = 1; indice <= CantidadDeFotos; indice++)
                {
                    objString += $"|Foto{indice}:" + metadataFotos[indice - 1].ToString();
                }
            }
            objString += "|Id:" + Id;
            return objString;
        }

        public void ToObjeto(string objString)
        {
            string buscadoCuerpo = "|Cuerpo:";
            string buscadoUsuarioId = "|UsuarioId:";
            string buscadoChipPadreId = "|ChipPadreId:";
            string buscadoCantidadDeFotos = "|CantidadDeFotos:";
            string buscadoMetadataFotos = "|MetadataFotos:";
            string metadataBase = "|Foto";
            string buscadoId = "|Id:";
            int posUsuarioId = objString.IndexOf(buscadoUsuarioId);
            int posChipPadreId = objString.IndexOf(buscadoChipPadreId);
            int posCantidadDeFotos = objString.IndexOf(buscadoCantidadDeFotos);
            int posicionMetadataFoto = objString.IndexOf(buscadoMetadataFotos);
            int posId = objString.IndexOf(buscadoId);
            this.Cuerpo = objString.Substring(buscadoCuerpo.Length, posUsuarioId - buscadoCuerpo.Length);
            this.UsuarioId = int.Parse(objString.Substring(posUsuarioId + buscadoUsuarioId.Length, posChipPadreId - buscadoUsuarioId.Length - posUsuarioId));
            if (posicionMetadataFoto > -1)
            {
                this.CantidadDeFotos = int.Parse(objString.Substring(posCantidadDeFotos + buscadoCantidadDeFotos.Length, posicionMetadataFoto - posCantidadDeFotos - buscadoCantidadDeFotos.Length));
                string metadata;
                string buscadoMetadata;
                List<MetadataFotoDTO> metadataFotos = new List<MetadataFotoDTO>();
                string buscadoMetadataSiguienteFoto; ;
                int posMetadataFotoSiguiente;
                int posicionMetadata;
                for (int indice = 1; indice <= this.CantidadDeFotos; indice++)
                {
                    buscadoMetadata = metadataBase + indice + ":";
                    posicionMetadata = objString.IndexOf(buscadoMetadata);
                    buscadoMetadataSiguienteFoto = metadataBase + (indice + 1) + ":";
                    if (indice == this.CantidadDeFotos)
                    {
                        posMetadataFotoSiguiente = posId;
                    }
                    else
                    {
                        posMetadataFotoSiguiente = objString.IndexOf(buscadoMetadataSiguienteFoto);
                    }
                    metadata = objString.Substring(posicionMetadata + buscadoMetadata.Length, posMetadataFotoSiguiente - posicionMetadata - buscadoMetadata.Length);
                    MetadataFotoDTO metadataFotoDTO = new MetadataFotoDTO();
                    metadataFotoDTO.ToObjeto(metadata);
                    metadataFotos.Add(metadataFotoDTO);
                }
                this.metadataFotos = metadataFotos;
                this.ChipPadreId = int.Parse(objString.Substring(posChipPadreId + buscadoChipPadreId.Length, posCantidadDeFotos - posChipPadreId - buscadoChipPadreId.Length));

            }
            else
            {
                this.CantidadDeFotos = 0;
                this.metadataFotos = new List<MetadataFotoDTO>();
                this.ChipPadreId = int.Parse(objString.Substring(posChipPadreId + buscadoChipPadreId.Length, posCantidadDeFotos - posChipPadreId - buscadoChipPadreId.Length));
            }
            this.Id = int.Parse(objString.Substring(posId + buscadoId.Length));
        }
    }
}
