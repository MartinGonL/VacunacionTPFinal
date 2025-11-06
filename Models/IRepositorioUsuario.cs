using VacunacionTPFinal.Models;
namespace VacunacionTPFinal.Models
{
    public interface IRepositorioUsuario
    {
        int Alta(Usuario usuario);
        int Modificacion(Usuario usuario);
        Usuario ObtenerPorId(int id);

        Usuario ObtenerPorEmail(string email);

        int ModificarAvatar(int idUsuario, string avatarUrl);
    }
}