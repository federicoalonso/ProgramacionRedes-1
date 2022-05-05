using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    internal class PantallaCliente
    {
        private static PantallaCliente Instancia = null;
        private static readonly object CandadoInstancia = new Object();

        private PantallaCliente() { }

        internal static PantallaCliente ObtenerInstancia()
        {
            if (PantallaCliente.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (PantallaCliente.Instancia == null)
                    {
                        PantallaCliente.Instancia = new PantallaCliente();
                    }
                }
            }
            return PantallaCliente.Instancia;
        }

        internal void Imprimir(string mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}
