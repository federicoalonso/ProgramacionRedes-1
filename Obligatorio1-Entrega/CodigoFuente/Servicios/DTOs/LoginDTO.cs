using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.DTOs
{
    public class LoginDTO
    {
        public string NombreDeUsuario { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            string objString = "";
            objString += "|NombreDeUsuario:" + NombreDeUsuario;
            objString += "|Password:" + Password;
            return objString;
        }

        public void ToObjeto(string objString)
        {
            string buscadoNombre = "|NombreDeUsuario:";
            string buscadoPassword = "|Password:";
            int posPass = objString.IndexOf(buscadoPassword);
            this.NombreDeUsuario = objString.Substring(buscadoNombre.Length, posPass - buscadoNombre.Length);
            this.Password = objString.Substring(posPass + buscadoPassword.Length);
        }
    }
}
