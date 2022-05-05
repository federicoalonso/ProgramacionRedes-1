using Servicios;
using Servicios.DTOs;
using Servicios.Protocolo;
using Servidor.Dominio;
using Servidor.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.LogicaNegocio
{
    internal class GestorChip
    {
        private RepositorioChip _repositorioChip = RepositorioChip.ObtenerInstancia();
        private const int _largoMinimoChip = 1;
        private const int _largoMaximoChip = 280;

        internal ResultadoOperacion AltaChip(Chip chip)
        {
            ResultadoOperacion retorno = Validaciones.ValidarLargoTexto(chip.Cuerpo, _largoMaximoChip, _largoMinimoChip, "Contenido del Chip");
            if (retorno.Codigo < CodigosConstantes.Error)
            {
                if(chip.ChipPadreId > 0)
                {
                    Chip? chipPadre = _repositorioChip.BuscarChipPorId(chip.ChipPadreId);
                    if (chipPadre == null)
                    {
                        retorno.Codigo = CodigosConstantes.ElementoNoEncontrado;
                        retorno.Mensaje = "No se encontró el chip que desea contestar.";
                        retorno.Entidad = null;
                        return retorno;
                    }
                }
            } 
            if(retorno.Codigo >= CodigosConstantes.Error)
            {
                retorno.Entidad = null;
                return retorno;
            }
            retorno.Entidad = _repositorioChip.Alta(chip);
            retorno.Mensaje = "Chip generado con éxito.";
            return retorno;
        }

        internal ResultadoOperacion BuscarChipsPorPalabra(BusquedaChipsDTO busquedaChipsDTO)
        {
            ResultadoOperacion resultado = new ResultadoOperacion()
            {
                Codigo = CodigosConstantes.TransmisionOK,
                Mensaje = "",
                Entidad = _repositorioChip.BuscarChipsPorPalabra(busquedaChipsDTO)
            };
            return resultado;
        }

        internal int CantidadChipsPorUsuario(int idUsuario)
        {
            int cantidad = 0;
            cantidad = _repositorioChip.CantidadChipsPorUsuario(idUsuario);
            return cantidad;
        }

        internal List<Chip> ChipsPorUsuario(int usuarioId)
        {
            return _repositorioChip.ChipsPorUsuario(usuarioId);
        }

        internal List<Chip> ChipsPorPeriodoDeTiempo(Double periodo)
        {
            return _repositorioChip.ObtenerChipsPorPeriodoDeTiempo(periodo);
        }
    }
}
