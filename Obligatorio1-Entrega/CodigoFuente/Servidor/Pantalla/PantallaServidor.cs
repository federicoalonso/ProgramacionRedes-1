using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{

    internal class PantallaServidor 
    {
        private static PantallaServidor Instancia = null;
        private static readonly object CandadoInstancia = new Object();

        private PantallaServidor() { }

        internal static PantallaServidor ObtenerInstancia()
        {
            if (PantallaServidor.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (PantallaServidor.Instancia == null)
                    {
                        PantallaServidor.Instancia = new PantallaServidor();
                    }
                }
            }
            return PantallaServidor.Instancia;
        }

        internal void Imprimir(string mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}
