using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.DTOs
{
    public class BusquedaChipsDTO
    {
        public string CadenaBuscada { get; set; }

        public override string ToString()
        {
            string objString = "";
            objString += "|Cadena:" + CadenaBuscada;
            return objString;
        }

        public void ToObjeto(string objString)
        {
            string buscadoCadena = "|Cadena:";
            this.CadenaBuscada = objString.Substring(buscadoCadena.Length, objString.Length - buscadoCadena.Length);
        }
    }
}
