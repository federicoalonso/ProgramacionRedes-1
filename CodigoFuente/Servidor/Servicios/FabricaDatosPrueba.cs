using Servidor.Dominio;
using Servidor.LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Servicios
{
    internal class FabricaDatosPrueba
    {
        GestorUsuario _gestorUsuario = new GestorUsuario();
        GestorChip _gestorChip = new GestorChip();

        internal void CargarDatosDePrueba()
        {
            CargarUsuarios();
            CargarChips();
            CargarRelaciones();
        }

        private void CargarRelaciones()
        {
            _gestorUsuario.SeguirUsuario(1,2);
            _gestorUsuario.SeguirUsuario(1,3);
            _gestorUsuario.SeguirUsuario(1,4);
            _gestorUsuario.SeguirUsuario(2,1);
            _gestorUsuario.SeguirUsuario(2,3);
            _gestorUsuario.SeguirUsuario(3,1);
            _gestorUsuario.SeguirUsuario(4,1);
            _gestorUsuario.SeguirUsuario(4,2);
            _gestorUsuario.SeguirUsuario(4,3);
            _gestorUsuario.SeguirUsuario(5,1);
            _gestorUsuario.SeguirUsuario(5,2);
        }

        private void CargarChips()
        {
            List<string> frases = new List<string>();
            frases.Add("CUANDO ME AMÉ DE VERDAD, COMPRENDÍ QUE EN CUALQUIER CIRCUNSTANCIA, YO ESTABA EN EL LUGAR CORRECTO Y EN EL MOMENTO PRECISO. Y, ENTONCES, PUDE RELAJARME. HOY SÉ QUE ESO TIENE NOMBRE: AUTOESTIMA");
            frases.Add("CON AMOR Y PACIENCIA NADA ES IMPOSIBLE.");
            frases.Add("LA MAYOR DECLARACIÓN DE AMOR ES LA QUE NO SE HACE; EL HOMBRE QUE SIENTE MUCHO, HABLA POCO.");
            frases.Add("PARA MI, EL MAYOR PECADO DE TODOS LOS PECADOS ES RECIBIR UN DON Y NO CULTIVARLO PARA QUE CREZCA, YA QUE EL TALENTO ES UN REGALO DIVINO.");
            frases.Add("NI PAGÁNDOME MIL MILLONES IBA A IR AL MADRID. PORQUE NO IBA A SER FELIZ. NO SOY UN CHICO DE PROMETER CINCUENTA GOLES, LO QUE PUEDO PROMETER ES CORRER COMO UN NEGRO PARA MAÑANA VIVIR COMO UN BLANCO.");
            frases.Add("PASO A PASO. NO CONCIBO NINGUNA OTRA MANERA PARA LOGRAR LAS COSAS");
            frases.Add("IRÉ A CUALQUIER PARTE, SIEMPRE QUE SEA HACIA ADELANTE.");

            for(int i = 0; i < 50; i++)
            {
                Chip chip = new Chip()
                {
                    CantidadDeFotos = 0,
                    Cuerpo = frases[i % 7],
                    UsuarioId = (i % 6) + 1,
                };
                _gestorChip.AltaChip(chip);
            }
        }

        private void CargarUsuarios()
        {
            Usuario usuario = new Usuario()
            {
                NombreReal = "Horacio Avalos",
                NombreUsuario = "havalos",
                Contrasenia = "password123456",
                Seguidores = new List<Usuario>(),
                Seguidos = new List<Usuario>()
            };
            _gestorUsuario.AltaUsuario(usuario);
            Usuario usuario1 = new Usuario()
            {
                NombreReal = "Federico Alonso",
                NombreUsuario = "falonso",
                Contrasenia = "password123456",
                Seguidores = new List<Usuario>(),
                Seguidos = new List<Usuario>()
            };
            _gestorUsuario.AltaUsuario(usuario1);
            Usuario usuario2 = new Usuario()
            {
                NombreReal = "Micaela Olivera",
                NombreUsuario = "molivera",
                Contrasenia = "password123456",
                Seguidores = new List<Usuario>(),
                Seguidos = new List<Usuario>()
            };
            _gestorUsuario.AltaUsuario(usuario2);
            Usuario usuario3 = new Usuario()
            {
                NombreReal = "Teresa Alonso",
                NombreUsuario = "talonso",
                Contrasenia = "password123456",
                Seguidores = new List<Usuario>(),
                Seguidos = new List<Usuario>()
            };
            _gestorUsuario.AltaUsuario(usuario3);
            Usuario usuario4 = new Usuario()
            {
                NombreReal = "Emiliano Marone",
                NombreUsuario = "emarona",
                Contrasenia = "password123456",
                Seguidores = new List<Usuario>(),
                Seguidos = new List<Usuario>()
            };
            _gestorUsuario.AltaUsuario(usuario4);
            Usuario usuario5 = new Usuario()
            {
                NombreReal = "Carina Davila",
                NombreUsuario = "cdavila",
                Contrasenia = "password123456",
                Seguidores = new List<Usuario>(),
                Seguidos = new List<Usuario>()
            };
            _gestorUsuario.AltaUsuario(usuario5);
        }

    }
}
