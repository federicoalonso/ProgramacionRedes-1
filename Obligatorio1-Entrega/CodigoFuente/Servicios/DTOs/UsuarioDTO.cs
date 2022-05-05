using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.DTOs
{
    public class UsuarioDTO
    {
        public string NombreUsuario { get; set; }
        public string NombreReal { get; set; }
        public string NombreDeFoto { set; get; }
        public string Contrasenia { get; set; }
        public int Id { get; set; }
        public int Seguidores { get; set; }
        public int Seguidos { get; set; }
        public int CantidadChips { get; set; }
        public List<ChipDTO> Chips { get; set; }

        private const string _separador = "{*}";

        public UsuarioDTO()
        {
            Chips = new List<ChipDTO>();
            CantidadChips = 0;
        }

        public override string ToString()
        {
            string objString = "";
            objString += "|NombreUsuario:" + NombreUsuario;
            objString += "|NombreReal:" + NombreReal;
            objString += "|NombreDeFoto:" + NombreDeFoto;
            objString += "|Password:" + Contrasenia;
            objString += "|Seguidores:" + Seguidores;
            objString += "|Seguidos:" + Seguidos;
            objString += "|Id:" + Id;
            objString += "|CantidadChips:" + CantidadChips;
            objString += "|Chips:";
            foreach(ChipDTO chip in Chips)
            {
                objString += chip.ToString() + _separador;
            }
            if(CantidadChips > 0)
                objString = objString.Remove(objString.Length - _separador.Length);
            return objString;
        }

        public void ToObjeto(string objString)
        {
            string buscadoNombre = "|NombreUsuario:";
            string buscadoNombreReal = "|NombreReal:";
            string buscadoNombreDeFoto = "|NombreDeFoto:";
            string buscadoPassword = "|Password:";
            string buscadoSeguidores = "|Seguidores:";
            string buscadoSeguidos = "|Seguidos:";
            string buscadoId = "|Id:";
            string buscadoCantChips = "|CantidadChips:";
            string buscadoChips = "|Chips:";
            int posNomR = objString.IndexOf(buscadoNombreReal);
            int posNomF = objString.IndexOf(buscadoNombreDeFoto);
            int posPass = objString.IndexOf(buscadoPassword);
            int posSeguidores = objString.IndexOf(buscadoSeguidores);
            int posSeguidos = objString.IndexOf(buscadoSeguidos);
            int posId = objString.IndexOf(buscadoId);
            int posCantChips = objString.IndexOf(buscadoCantChips);
            int posChips = objString.IndexOf(buscadoChips);
            this.NombreUsuario = objString.Substring(buscadoNombre.Length, posNomR - buscadoNombre.Length);
            this.NombreReal = objString.Substring(posNomR + buscadoNombreReal.Length, posNomF - buscadoNombreReal.Length - posNomR);
            this.NombreDeFoto = objString.Substring(posNomF + buscadoNombreDeFoto.Length, posPass - buscadoNombreDeFoto.Length - posNomF);
            this.Contrasenia = objString.Substring(posPass + buscadoPassword.Length, posSeguidores - buscadoPassword.Length - posPass);
            this.Seguidores = int.Parse(objString.Substring(posSeguidores + buscadoSeguidores.Length, posSeguidos - buscadoSeguidores.Length - posSeguidores));
            this.Seguidos = int.Parse(objString.Substring(posSeguidos + buscadoSeguidos.Length, posId - buscadoSeguidos.Length - posSeguidos));
            this.Id = int.Parse(objString.Substring(posId + buscadoId.Length, posCantChips - buscadoId.Length - posId));
            this.CantidadChips = int.Parse(objString.Substring(posCantChips + buscadoCantChips.Length, posChips - buscadoCantChips.Length - posCantChips));

            string cadena = objString.Substring(posChips + buscadoChips.Length);

            if (cadena != "")
            {
                List<string> resultado = cadena.Split(_separador).ToList();
                foreach (string str in resultado)
                {
                    ChipDTO chipDTO = new ChipDTO();
                    chipDTO.ToObjeto(str);
                    this.Chips.Add(chipDTO);
                }
            }

        }
    }
}
