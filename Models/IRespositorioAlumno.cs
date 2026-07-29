public interface IRepositorioAlumno
{
    int Alta(Alumno alumno);
    int Baja(int id);
    int Modificar(Alumno alumno);
    IEnumerable<Alumno> ObtenerTodos();
    // Paginado para la consigna
    IEnumerable<Alumno> ObtenerPaginados(int pagina, int cantidad);
    int ObtenerTotal();
    Alumno? ObtenerPorId(int id);
    IEnumerable<Alumno> ObtenerPorEscuelaId(int escuelaId);
    IEnumerable<Alumno> ObtenerPaginadosPorEscuelaId(int escuelaId, int pagina, int cantidad, string? buscar = null);
    int ObtenerTotalPorEscuelaId(int escuelaId, string? buscar = null);
}