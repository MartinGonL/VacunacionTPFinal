using VacunacionTPFinal.Models;
using System.Collections.Generic;

namespace VacunacionTPFinal.Models
{
    public interface IRepositorioVacuna
    {
        int Alta(Vacuna vacuna);
        int Baja(int id);
        int Modificacion(Vacuna vacuna);
        Vacuna ObtenerPorId(int id);
        IList<Vacuna> ObtenerTodos();

        IList<Vacuna> BuscarPorNombre(string nombre);
    }
}