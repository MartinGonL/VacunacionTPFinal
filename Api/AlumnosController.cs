using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VacunacionTPFinal.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly IRepositorioAlumno _repoAlumno;

        public AlumnosController(IRepositorioAlumno repoAlumno)
        {
            _repoAlumno = repoAlumno;
        }

        // GET: api/alumnos?pagina=1&q=...&escuelaId=...&turno=...&grado=...
        [HttpGet]
        public IActionResult Get([FromQuery] int pagina = 1, [FromQuery] string? q = null, [FromQuery] int? escuelaId = null, [FromQuery] string? turno = null, [FromQuery] string? grado = null)
        {
            try
            {
                int cantidadPorPagina = 10;

                if (escuelaId.HasValue)
                {
                    var totalEscuela = _repoAlumno.ObtenerTotalPorEscuelaId(escuelaId.Value, q, turno, grado);
                    var totalPaginasEscuela = (int)Math.Ceiling((double)totalEscuela / cantidadPorPagina);
                    var listaEscuela = _repoAlumno.ObtenerPaginadosPorEscuelaId(escuelaId.Value, pagina, cantidadPorPagina, q, turno, grado);

                    return Ok(new
                    {
                        PaginaActual = pagina,
                        TotalPaginas = totalPaginasEscuela,
                        TotalAlumnos = totalEscuela,
                        Resultados = listaEscuela
                    });
                }

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var termino = q.Trim().ToLower();
                    var resultados = _repoAlumno.ObtenerTodos()
                        .Where(a => a.Nombre.ToLower().Contains(termino) ||
                                    a.Apellido.ToLower().Contains(termino) ||
                                    a.DNI.Contains(termino))
                        .ToList();
                    return Ok(new
                    {
                        PaginaActual = 1,
                        TotalPaginas = 1,
                        TotalAlumnos = resultados.Count,
                        Resultados = resultados
                    });
                }

                var total = _repoAlumno.ObtenerTotal();
                var totalPaginas = (int)Math.Ceiling((double)total / cantidadPorPagina);
                var lista = _repoAlumno.ObtenerPaginados(pagina, cantidadPorPagina);

                return Ok(new
                {
                    PaginaActual = pagina,
                    TotalPaginas = totalPaginas,
                    TotalAlumnos = total,
                    Resultados = lista
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/alumnos/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var alumno = _repoAlumno.ObtenerPorId(id);
                if (alumno == null) return NotFound();
                return Ok(alumno);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/alumnos
        [HttpPost]
        public IActionResult Post([FromBody] Alumno alumno)
        {
            try
            {
                ModelState.Remove(nameof(alumno.Escuela));

                if (string.IsNullOrWhiteSpace(alumno.Turno) || (alumno.Turno != "Mañana" && alumno.Turno != "Tarde"))
                {
                    return BadRequest("Debe seleccionar un turno válido (Turno Mañana o Turno Tarde).");
                }

                // Cálculo automático del Grado en base a la Fecha de Nacimiento
                alumno.Grado = Alumno.CalcularGradoSegunFecha(alumno.FechaNacimiento);

                if (ModelState.IsValid)
                {
                    var existente = _repoAlumno.ObtenerTodos().FirstOrDefault(a => a.DNI == alumno.DNI);
                    if (existente != null)
                    {
                        return BadRequest("Ya existe un alumno registrado con ese DNI.");
                    }

                    var id = _repoAlumno.Alta(alumno);
                    var creado = _repoAlumno.ObtenerPorId(id);
                    return Ok(creado);
                }
                
                var errs = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/alumnos/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Alumno alumno)
        {
            try
            {
                ModelState.Remove(nameof(alumno.Escuela));

                if (string.IsNullOrWhiteSpace(alumno.Turno) || (alumno.Turno != "Mañana" && alumno.Turno != "Tarde"))
                {
                    return BadRequest("Debe seleccionar un turno válido (Turno Mañana o Turno Tarde).");
                }

                // Cálculo automático del Grado en base a la Fecha de Nacimiento
                alumno.Grado = Alumno.CalcularGradoSegunFecha(alumno.FechaNacimiento);

                if (ModelState.IsValid)
                {
                    var existente = _repoAlumno.ObtenerTodos().FirstOrDefault(a => a.DNI == alumno.DNI && a.AlumnoID != id);
                    if (existente != null)
                    {
                        return BadRequest("Ya existe otro alumno registrado con ese DNI.");
                    }

                    alumno.AlumnoID = id;
                    _repoAlumno.Modificar(alumno);
                    var actualizado = _repoAlumno.ObtenerPorId(id);
                    return Ok(actualizado);
                }

                var errs = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/alumnos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existente = _repoAlumno.ObtenerPorId(id);
                if (existente == null) return NotFound();

                _repoAlumno.Baja(id);
                return Ok(new { Mensaje = "Alumno eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
