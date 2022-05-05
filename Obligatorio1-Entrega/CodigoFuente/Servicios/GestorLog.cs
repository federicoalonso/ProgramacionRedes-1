namespace Servicios
{
    public class GestorLog
    {
        private static string _ruta = Path.GetDirectoryName(Directory.GetCurrentDirectory());
        private static string _nombre = "Log.txt";

        public GestorLog(string directorio, string nombreArchivo)
        {
            _ruta = directorio;
            _nombre = nombreArchivo;
        }

        public static void EscribirLog(string log)
        {
            using (StreamWriter w = File.AppendText(_ruta + "\\" + _nombre))
            {
                Log(log, w);
            }
        }

        private static void Log(string log, StreamWriter txtWriter)
        {
            txtWriter.Write("\r\nEntrada de Log: ");
            txtWriter.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            txtWriter.WriteLine("  :");
            txtWriter.WriteLine($"  :{log}");
            txtWriter.WriteLine("-------------------------------");
        }
    }
}
