using Servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class ResultadoOperacion
    {
        public string Mensaje { get; set; }
        public object Entidad { get; set; }
        public int Codigo { get; set; }
        public string Tipo { get; set; }
        private const string _separador = "{*}";

        public override string ToString()
        {
            string objString = "";
            objString += "|Mensaje:" + Mensaje;
            objString += "|Codigo:" + Codigo;
            objString += "|Tipo:" + Tipo;
            if(Entidad != null)
            {
                objString += "|Entidad:";
                if (Tipo == "ListUsuarioDTO")
                {
                    List<UsuarioDTO> usus = (List<UsuarioDTO>)Entidad;
                    foreach (UsuarioDTO usuario in usus)
                    {
                        objString += usuario.ToString() + _separador;
                    }
                    if(usus.Count()>0)
                        objString = objString.Remove(objString.Length - 3);
                }
                else if (Tipo == "ListChipDTO")
                {
                    List<ChipDTO> chips = (List<ChipDTO>)Entidad;
                    foreach (ChipDTO chipDTO in chips)
                    {
                        objString += chipDTO.ToString() + _separador;
                    }
                    if (chips.Count() > 0)
                        objString = objString.Remove(objString.Length - 3);
                }
                else
                {
                    objString += Entidad.ToString();
                }
            }
            else
            {
                objString += "|Entidad:null";
            }
            return objString;
        }

        public void ToObject(string objS)
        {
            string buscadoMensaje = "|Mensaje:";
            string buscadoCodigo = "|Codigo:";
            string buscadoTipo = "|Tipo:";
            string buscadoEntidad = "|Entidad:";
            int posCod = objS.IndexOf(buscadoCodigo);
            int posTip = objS.IndexOf(buscadoTipo);
            int posEnt = objS.IndexOf(buscadoEntidad);

            this.Mensaje = objS.Substring(buscadoMensaje.Length, posCod - buscadoMensaje.Length);
            string codigoS = objS.Substring(posCod + buscadoCodigo.Length, posTip - posCod - buscadoCodigo.Length);
            this.Codigo = int.Parse(codigoS);
            this.Tipo = objS.Substring(posTip + buscadoTipo.Length, posEnt - posTip - buscadoTipo.Length);

            string ent = objS.Substring(posEnt + buscadoEntidad.Length);

            Object entidad = ConvertirATipos(ent, this.Tipo);

            this.Entidad = entidad;
        }

        private Object ConvertirATipos(string cadena, string tipo)
        {
            Object obj = null;
            if (cadena == "")
                return obj;
            switch (tipo)
            {
                case "LoginDTO":
                    LoginDTO log = new LoginDTO();
                    log.ToObjeto(cadena);
                    obj = log;
                    break;
                case "UsuarioDTO":
                    UsuarioDTO usu = new UsuarioDTO();
                    usu.ToObjeto(cadena);
                    obj = usu;
                    break;
                case "ListUsuarioDTO":
                    List<UsuarioDTO> usuariosDTO = new List<UsuarioDTO>();
                    List<string> result = cadena.Split(_separador).ToList();
                    foreach (string str in result)
                    {
                        UsuarioDTO usuario = new UsuarioDTO();
                        usuario.ToObjeto(str);
                        usuariosDTO.Add(usuario);
                    }
                    obj = usuariosDTO;
                    break;
                case "ChipDTO":
                    ChipDTO chip = new ChipDTO();
                    chip.ToObjeto(cadena);
                    obj=chip;
                    break;
                case "ListChipDTO":
                    List<ChipDTO> chipsDTO = new List<ChipDTO>();
                    List<string> resultado = cadena.Split(_separador).ToList();
                    foreach (string str in resultado)
                    {
                        ChipDTO chipDTO= new ChipDTO();
                        chipDTO.ToObjeto(str);
                        chipsDTO.Add(chipDTO);
                    }
                    obj = chipsDTO;
                    break;
                case "Error":
                    break;
                case "OKRespuesta":
                    break;
                default:
                    break;
            }
            return obj;
        }
    }
}
