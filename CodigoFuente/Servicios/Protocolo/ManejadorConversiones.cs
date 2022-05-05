using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Protocolo
{
    public static class ManejadorConversiones
    {
        public static byte[] ConvertirStringABytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ConvertirBytesAString(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        public static byte[] ConvertirIntABytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static int ConvertirBytesAInt(byte[] value)
        {
            return BitConverter.ToInt32(value);
        }

        public static byte[] ConvertirLongABytes(long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static long ConvertirBytesALong(byte[] value)
        {
            return BitConverter.ToInt64(value);
        }
    }
}
