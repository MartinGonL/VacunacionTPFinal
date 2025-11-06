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
        [Authorize(Roles = "Administrador, Agente")]
        public IActionResult Index()
        {            
            var lista = repoRegistro.ObtenerPaginado(1, 50); 
            return View(lista);
        }

        // Muestra el historial de un alumno específico
        public IActionResult HistorialAlumno(int id) 
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
            ViewBag.Alumnos = new SelectList(repoAlumno.ObtenerTodos(), "ID_Alumno", "NombreCompleto");
            ViewBag.Vacunas = new SelectList(repoVacuna.ObtenerTodos(), "ID_Vacuna", "Nombre");
        }

        // GET: RegistroVacunacion/Create
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
                        TempData["MensajeError"] = "Su usuario no es un Agente Sanitario válido.";
                        CargarViewBags();
                        return View(registro);
                    }
                    
                    registro.ID_Agente = int.Parse(agenteClaim.Value);

                    registro.FechaAplicacion = DateTime.Now;

                    repoRegistro.Alta(registro);

                    TempData["MensajeExito"] = "Vacunación registrada con éxito.";
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