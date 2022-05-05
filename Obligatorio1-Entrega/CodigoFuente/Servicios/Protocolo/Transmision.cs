using System.Text;

namespace Servicios.Protocolo
{
    public class Transmision
    {
        public byte[] BodyByte { get; }
        public byte[] HeaderByte { get; }
        public Header HeaderT { get; set; }
        public string BodyT { get; set; }

        public Transmision() { }
        public Transmision(string direccion, int comando, string msj = "")
        {
            BodyT = msj;
            BodyByte = Encoding.UTF8.GetBytes(msj);
            int msgLength = BodyByte.Length;
            HeaderT = new Header(direccion, comando, msgLength);
            HeaderByte = HeaderT.ObtenerRequest();
        }
    }
}
