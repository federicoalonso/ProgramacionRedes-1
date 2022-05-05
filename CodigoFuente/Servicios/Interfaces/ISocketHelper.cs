namespace Servicios.Interfaces
{
    public interface ISocketHelper
    {
        public void Send(byte[] data);
        public byte[] Receive(int length);
        public void Close();
    }
}
