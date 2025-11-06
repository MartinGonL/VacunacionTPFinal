using VacunacionTPFinal.Models;
using System.Collections.Generic;

namespace VacunacionTPFinal.Models
{
    public interface IRepositorioRegistroVacunacion
    {
        int Alta(RegistroVacunacion registro);
        int Baja(int id);
        int Modificacion(RegistroVacunacion registro);
        RegistroVacunacion ObtenerPorId(int id);

        IList<RegistroVacunacion> ObtenerPorAlumno(int idAlumno);

        IList<RegistroVacunacion> ObtenerPorAgente(int idAgente);
        IList<RegistroVacunacion> ObtenerPorEscuela(int idEscuela);

        IList<RegistroVacunacion> ObtenerPaginado(int pagina, int cantidad);

        int ContarTotal();
    }
}