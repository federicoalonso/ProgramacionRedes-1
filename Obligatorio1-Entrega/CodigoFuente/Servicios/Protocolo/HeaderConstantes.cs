using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Protocolo
{
    public static class HeaderConstantes
    {
        public static string Request = "REQ";
        public static string Response = "RES";
        public static int ComandosLength = 2;
        public static int DataLength = 4;
        public const int LargoFijoArchivo = 8;
        public const int LargoMaximoDatos = 33333;

        public static long CalcularParticionesDeArchivo(long tamanioArchivo)
        {
            var particionesDeArchivo = tamanioArchivo / LargoMaximoDatos;
            return particionesDeArchivo * LargoMaximoDatos == tamanioArchivo ? particionesDeArchivo : particionesDeArchivo + 1;
        }
    }
}
