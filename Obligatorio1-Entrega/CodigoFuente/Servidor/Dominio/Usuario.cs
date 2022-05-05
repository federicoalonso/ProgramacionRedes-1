namespace Servidor.Dominio
{
    internal class Usuario
    {
        internal int Id { get; set; }
        internal string NombreUsuario { get; set; }
        internal string NombreReal { get; set; }
        internal string Contrasenia { get; set; }
        internal bool Bloqueado { get; set; }
        internal string NombreDeFoto { get; set; }
        internal List<Usuario> Seguidores { get; set; }
        internal List<Usuario> Seguidos { get; set; }

        internal Usuario()
        {
            Seguidores = new List<Usuario>();
            Seguidos = new List<Usuario>();
        }

        public override string ToString()
        {
            return "Usuario ID " + this.Id + " -\t" + this.NombreUsuario
                + " -\t" + this.NombreReal + " -\t" + this.Seguidores.Count
                + " -\t" + this.Seguidos.Count + " -\t" + this.Bloqueado;
        }
    }
}
