using VacunacionTPFinal.Models;
using System.Collections.Generic;

namespace VacunacionTPFinal.Models
{
    public interface IRepositorioAlumno
    {
        int Alta(Alumno alumno);
        int Baja(int id);
        int Modificacion(Alumno alumno);
        Alumno ObtenerPorId(int id);
        IList<Alumno> ObtenerTodos();

       
        IList<Alumno> ObtenerPorEscuela(int idEscuela);

        IList<Alumno> ObtenerPaginado(int pagina, int cantidad);

        
        int ContarTotal();
    }
}