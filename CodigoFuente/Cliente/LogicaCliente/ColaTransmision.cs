using Servicios.Protocolo;

namespace Cliente.LogicaCliente
{
    internal class ColaTransmision
    {
        private List<Transmision> _transmisiones = new List<Transmision>();
        private static ColaTransmision Instancia = null;
        private static readonly object CandadoInstancia = new Object();
        private static readonly object CandadoTransisiones = new Object();

        private ColaTransmision()
        {
            _transmisiones = new List<Transmision>();
        }
        internal static ColaTransmision ObtenerInstancia()
        {
            if (ColaTransmision.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (ColaTransmision.Instancia == null)
                    {
                        ColaTransmision.Instancia = new ColaTransmision();
                    }
                }
            }
            return ColaTransmision.Instancia;
        }

        internal void PushTransmision(Transmision t)
        {
            lock (CandadoTransisiones)
            {
                _transmisiones.Add(t);
            }
        }

        internal Transmision? PopTransmision()
        {
            lock (CandadoTransisiones)
            {
                Transmision? t = _transmisiones.FirstOrDefault();
                if(t != null)
                {
                    _transmisiones.Remove(t);
                }
                return t;
            }
        }
    }
}
