using Cliente;
using Cliente.Menus;

IniciarConsola();

void IniciarConsola()
{
    EstadoCliente appCliente = EstadoCliente.ObtenerInstancia();
    MenuUsuarioDesconectado menuDesconectado = new MenuUsuarioDesconectado();
    new Thread(() => menuDesconectado.Menu()).Start();
}