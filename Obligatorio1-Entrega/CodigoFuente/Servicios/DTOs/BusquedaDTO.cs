using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.DTOs
{
    public class BusquedaDTO
    {
        public string NombreReal { get; set; }
        public string NombreUsuario { get; set; }
        public bool Inclusivo { get; set; }

        public override string ToString()
        {
            string objString = "";
            objString += "|NombreUsuario:" + NombreUsuario;
            objString += "|NombreReal:" + NombreReal;
            objString += "|Inclusivo:" + Inclusivo;
            return objString;
        }

        public void ToObjeto(string objString)
        {
            string buscadoNombre = "|NombreUsuario:";
            string buscadoNombreReal = "|NombreReal:";
            string buscadoInclusivo = "|Inclusivo:";
            int posNomR = objString.IndexOf(buscadoNombreReal);
            int posIn = objString.IndexOf(buscadoInclusivo);
            this.NombreUsuario = objString.Substring(buscadoNombre.Length, posNomR - buscadoNombre.Length);
            this.NombreReal = objString.Substring(posNomR + buscadoNombreReal.Length, posIn - buscadoNombreReal.Length - posNomR);
            this.Inclusivo = bool.Parse(objString.Substring(posIn + buscadoInclusivo.Length));
        }
    }
}
