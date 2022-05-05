using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.DTOs
{
    public class MetadataFotoDTO
    {
        public int LargoNombre { get; set; }
        public string Nombre { get; set; }
        public long TamanioArchivo { get; set; }

        public override string ToString()
        {
            string objString = "";
            objString += "|LargoNombre:" + LargoNombre.ToString();
            objString += "|Nombre:" + Nombre;
            objString += "|TamanioArchivo:" + TamanioArchivo.ToString();
            return objString;
        }

        public void ToObjeto(string objString)
        {
            string buscadoLargoNombre = "|LargoNombre:";
            string buscadoNombre = "|Nombre:";
            string buscadoTamanioArchivo = "|TamanioArchivo:";
            int posNombre = objString.IndexOf(buscadoNombre);
            int posTamanioArchivo = objString.IndexOf(buscadoTamanioArchivo);
            this.LargoNombre = int.Parse(objString.Substring(buscadoLargoNombre.Length, posNombre - buscadoLargoNombre.Length));
            this.Nombre = objString.Substring(posNombre + buscadoNombre.Length, posTamanioArchivo - buscadoNombre.Length - posNombre);
            this.TamanioArchivo = long.Parse(objString.Substring(posTamanioArchivo + buscadoTamanioArchivo.Length));
        }
    }
}
