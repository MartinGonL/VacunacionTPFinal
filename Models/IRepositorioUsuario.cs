using VacunacionTPFinal.Models;
namespace VacunacionEscolar.Repositorios
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