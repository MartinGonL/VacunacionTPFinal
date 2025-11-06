using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Security.Claims; // Para obtener el ID del Agente logueado
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Controllers
{
    [Authorize(Roles = "Agente, Administrador")]
    public class RegistroVacunacionController : Controller
    {
        private readonly IRepositorioRegistroVacunacion repoRegistro;
        private readonly IRepositorioAlumno repoAlumno;
        private readonly IRepositorioVacuna repoVacuna;
        private readonly IRepositorioAgenteSanitario repoAgente;

        public RegistroVacunacionController(IRepositorioRegistroVacunacion repoRegistro, IRepositorioAlumno repoAlumno, IRepositorioVacuna repoVacuna, IRepositorioAgenteSanitario repoAgente)
        {
            this.repoRegistro = repoRegistro;
            this.repoAlumno = repoAlumno;
            this.repoVacuna = repoVacuna;
            this.repoAgente = repoAgente;
        }

        // GET: RegistroVacunacion
        // Muestra la lista de TODAS las vacunaciones (para Admin)
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            // Aquí deberías implementar paginación (como pide la consigna)
            // Por ahora, traemos todos
            var lista = repoRegistro.ObtenerPaginado(1, 100); // Trae los 100 más recientes
            return View(lista);
        }

        // Muestra el historial de un alumno específico
        public IActionResult HistorialAlumno(int id) // id es ID_Alumno
        {
            var alumno = repoAlumno.ObtenerPorId(id);
            if (alumno == null) return NotFound();
            
            ViewData["Alumno"] = alumno;
            var historial = repoRegistro.ObtenerPorAlumno(id);
            return View(historial);
        }

        // Método privado para cargar los dropdowns
        private void CargarViewBags()
        {
            // Requisito: "Al seleccionar entidades... debe hacerse con búsqueda vía ajax"
            // Por ahora (para la entrega base), cargamos todos
            ViewBag.Alumnos = new SelectList(repoAlumno.ObtenerTodos(), "ID_Alumno", "NombreCompleto");
            ViewBag.Vacunas = new SelectList(repoVacuna.ObtenerTodos(), "ID_Vacuna", "Nombre");
        }

        // GET: RegistroVacunacion/Create
        // Esta es la acción de "Registrar Nueva Vacunación"
        public IActionResult Create()
        {
            CargarViewBags();
            return View();
        }

        // POST: RegistroVacunacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegistroVacunacion registro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 1. Obtener el ID del Agente que está logueado
                    var agenteClaim = User.FindFirst("AgenteId");
                    if (agenteClaim == null)
                    {
                        // Es un Admin sin agente asociado, o algo salió mal
                        TempData["MensajeError"] = "Su usuario no es un Agente Sanitario válido.";
                        CargarViewBags();
                        return View(registro);
                    }
                    
                    registro.ID_Agente = int.Parse(agenteClaim.Value);

                    // 2. Settear la fecha de aplicación
                    registro.FechaAplicacion = DateTime.Now;

                    // 3. Guardar el registro
                    repoRegistro.Alta(registro);

                    TempData["MensajeExito"] = "Vacunación registrada con éxito.";
                    // Redirigir al historial del alumno vacunado
                    return RedirectToAction(nameof(HistorialAlumno), new { id = registro.ID_Alumno });
                }
                
                CargarViewBags();
                return View(registro);
            }
            catch(Exception ex)
            {
                ViewData["MensajeError"] = $"Error al registrar: {ex.Message}";
                CargarViewBags();
                return View(registro);
            }
        }

        // (Aquí irían las acciones de Edit y Delete para los registros,
        // pero por lo general solo un Admin debería poder hacerlas)

        // GET: RegistroVacunacion/Delete/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var registro = repoRegistro.ObtenerPorId(id);
            if (registro == null)
            {
                return NotFound();
            }
            return View(registro);
        }

        // POST: RegistroVacunacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repoRegistro.Baja(id);
                TempData["MensajeExito"] = "Registro de vacunación eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["MensajeError"] = "Error al eliminar el registro.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}