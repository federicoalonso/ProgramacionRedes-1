using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Protocolo
{
    public class Header
    {
        private byte[] _direccion;
        private byte[] _comando;
        private byte[] _dataLength;

        private string _sDireccion;
        private int _iComando;
        private int _iDataLength;

        public string SDireccion
        {
            get => _sDireccion;
            set => _sDireccion = value;
        }
        public int IComando
        {
            get => _iComando;
            set => _iComando = value;
        }
        public int IDataLength
        {
            get => _iDataLength;
            set => _iDataLength = value;
        }

        public Header() { }
        public Header(string direccion, int comando, int dataLength)
        {
            _direccion = Encoding.UTF8.GetBytes(direccion);
            var sComando = comando.ToString("D2");
            _comando = Encoding.UTF8.GetBytes(sComando);
            var sDataLength = dataLength.ToString("D4");
            _dataLength = Encoding.UTF8.GetBytes(sDataLength);
        }

        public byte[] ObtenerRequest()
        {
            var header = new byte[HeaderConstantes.Request.Length + HeaderConstantes.ComandosLength + HeaderConstantes.DataLength];
            Array.Copy(_direccion, 0, header, 0, HeaderConstantes.Request.Length);
            Array.Copy(_comando, 0, header, HeaderConstantes.Request.Length, HeaderConstantes.ComandosLength);
            Array.Copy(_dataLength, 0, header, HeaderConstantes.Request.Length + HeaderConstantes.ComandosLength, HeaderConstantes.DataLength);
            return header;
        }

        public bool DecodificarData(byte[] data)
        {
            try
            {
                _sDireccion = Encoding.UTF8.GetString(data, 0, HeaderConstantes.Request.Length);
                var comando = Encoding.UTF8.GetString(data, HeaderConstantes.Request.Length, HeaderConstantes.ComandosLength);
                _iComando = int.Parse(comando);
                var dataLength = Encoding.UTF8.GetString(data, HeaderConstantes.Request.Length + HeaderConstantes.ComandosLength, HeaderConstantes.DataLength);
                _iDataLength = int.Parse(dataLength);
                return true;
            }catch(Exception ex)
            {
                GestorLog.EscribirLog(ex.Message);
                return false;
            }
        }
    }
}
