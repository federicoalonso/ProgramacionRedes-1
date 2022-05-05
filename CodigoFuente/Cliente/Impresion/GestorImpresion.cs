using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Impresion
{
    internal static class GestorImpresion
    {
        internal static void Imprimir(string salida)
        {
            PantallaCliente pantalla = PantallaCliente.ObtenerInstancia();
            pantalla.Imprimir(salida);
        }

        internal static PantallaCliente BloquearPantalla()
        {
            return PantallaCliente.ObtenerInstancia();
        }
    }
}
