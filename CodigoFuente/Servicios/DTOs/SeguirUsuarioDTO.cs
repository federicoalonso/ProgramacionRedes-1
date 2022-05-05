using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.DTOs
{
    public class SeguirUsuarioDTO
    {
        public int Seguido { get; set; }
        public int Seguidor { get; set; }

        public override string ToString()
        {
            string objString = "";
            objString += "|Seguido:" + Seguido;
            objString += "|Seguidor:" + Seguidor;
            return objString;
        }

        public void ToObjeto(string objString)
        {
            string buscadoSeguido = "|Seguido:";
            string buscadoSeguidor = "|Seguidor:";
            int posSegr = objString.IndexOf(buscadoSeguidor);
            this.Seguido = int.Parse(objString.Substring(buscadoSeguido.Length, posSegr - buscadoSeguido.Length));
            this.Seguidor = int.Parse(objString.Substring(posSegr + buscadoSeguidor.Length));
        }
    }
}
