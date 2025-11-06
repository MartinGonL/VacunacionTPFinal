using VacunacionTPFinal.Models;
using System.Collections.Generic;

namespace VacunacionEscolar.Repositorios
{
    
    public interface IRepositorioEscuela
    {
        int Alta(Escuela escuela);
        int Baja(int id);
        int Modificacion(Escuela escuela);
        Escuela ObtenerPorId(int id);
        IList<Escuela> ObtenerTodos();
        IList<Escuela> BuscarPorNombre(string nombre);
    }
}