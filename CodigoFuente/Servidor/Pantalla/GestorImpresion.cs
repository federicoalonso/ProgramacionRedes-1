using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Pantalla
{
    internal class GestorImpresion
    {
        internal static void Imprimir(string salida)
        {
            PantallaServidor pantalla = PantallaServidor.ObtenerInstancia();
            pantalla.Imprimir(salida);
        }

        internal static PantallaServidor BloquearPantalla()
        {
            return PantallaServidor.ObtenerInstancia();
        }
    }
}
