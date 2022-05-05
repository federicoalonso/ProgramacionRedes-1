using Servicios.DTOs;
using Servidor.Dominio;

namespace Servidor.Repositorio
{
    internal class RepositorioChip
    {
        private List<Chip> Chips { get; set; }
        private static int autonumerado = 0;
        private static RepositorioChip Instancia = null;
        private static readonly object CandadoInstancia = new Object();
        private static readonly object CandadoLista = new Object();

        private RepositorioChip()
        {
            Chips = new List<Chip>();
        }

        internal static RepositorioChip ObtenerInstancia()
        {
            if (RepositorioChip.Instancia == null)
            {
                lock (CandadoInstancia)
                {
                    if (RepositorioChip.Instancia == null)
                    {
                        RepositorioChip.Instancia = new RepositorioChip();
                    }
                }
            }
            return RepositorioChip.Instancia;
        }

        internal Chip? BuscarChipPorId(int chipPadreId)
        {
            return Chips.Where(c => c.Id == chipPadreId).FirstOrDefault();
        }

        internal Chip Alta(Chip chip)
        {
            Interlocked.Increment(ref autonumerado);
            chip.Id = autonumerado;
            lock (CandadoLista)
            {
                Chips.Add(chip);
            }
            return chip;
        }

        internal List<Chip> BuscarChipsPorPalabra(BusquedaChipsDTO busquedaChipsDTO)
        {
            List<Chip> chips;
            lock (CandadoLista)
            {
                chips = Chips.Where(chip => chip.Cuerpo.ToLower().Contains((busquedaChipsDTO.CadenaBuscada).Trim().ToLower())).ToList();
            }
            return chips;
        }

        internal int CantidadChipsPorUsuario(int idUsuario)
        {
            int cantidad = 0;
            lock (CandadoLista)
            {
                foreach(Chip chip in Chips)
                {
                    if(chip.UsuarioId == idUsuario)
                        cantidad++;
                }
            }
            return cantidad;
        }

        internal List<Chip> ChipsPorUsuario(int usuarioId)
        {
            List<Chip> chips = new List<Chip>();
            for(int i = Chips.Count() -1; i>= 0; i--)
            {
                Chip chip = Chips[i];
                if (chip.UsuarioId == usuarioId && chip.ChipPadreId == -1)
                {
                    List<Chip> parcial = new List<Chip>();
                    parcial.Add(chip);
                    ChipsRecurrente(parcial, chip);
                    foreach (Chip ch in parcial)
                    {
                        if (!chips.Contains(ch))
                            chips.Add(ch);
                    }
                }
            }
            return chips;
        }

        private List<Chip> ChipsRecurrente(List<Chip> lista, Chip chip)
        {
            for (int i = Chips.Count() -1; i >= 0; i--)
            {
                Chip ch = Chips[i];
                if (ch.ChipPadreId == chip.Id)
                {
                    lista.Add(ch);
                    ChipsRecurrente(lista, ch);
                }
            }
            return lista;
        }

        internal List<Chip> ObtenerChipsPorPeriodoDeTiempo(double periodo)
        {
            DateTime horaActual = DateTime.Now;
            var chipsDurantePeriodo = from chip in Chips
                                      where chip.Fecha.AddHours(periodo) >= horaActual                                            
                                      select chip;

            return chipsDurantePeriodo.ToList();
        }
    }
}
