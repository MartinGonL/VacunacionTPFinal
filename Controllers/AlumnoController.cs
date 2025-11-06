using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectList
using System;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly IRepositorioAlumno repoAlumno;
        private readonly IRepositorioEscuela repoEscuela;

        public AlumnoController(IRepositorioAlumno repoAlumno, IRepositorioEscuela repoEscuela)
        {
            this.repoAlumno = repoAlumno;
            this.repoEscuela = repoEscuela;
        }


        // GET: Alumno
        public IActionResult Index()
        {
            var lista = repoAlumno.ObtenerTodos();
            return View(lista);
        }

        // Método privado para cargar el ViewBag de Escuelas
        private void CargarEscuelasViewBag()
        {
            // Esto cumple el requisito de "seleccionar entidades relacionadas"
            var escuelas = repoEscuela.ObtenerTodos();
            ViewBag.Escuelas = new SelectList(escuelas, "ID_Escuela", "Nombre");
        }

        // GET: Alumno/Create
        public IActionResult Create()
        {
            CargarEscuelasViewBag();
            return View();
        }

        // POST: Alumno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Alumno alumno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repoAlumno.Alta(alumno);
                    TempData["MensajeExito"] = "Alumno registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                CargarEscuelasViewBag();
                return View(alumno);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Duplicate entry"))
                {
                    ViewData["MensajeError"] = "El DNI del alumno ya está registrado.";
                }
                else
                {
                    ViewData["MensajeError"] = $"Error: {ex.Message}";
                }
                CargarEscuelasViewBag();
                return View(alumno);
            }
        }

        // GET: Alumno/Edit/5
        [Authorize(Roles = "Administrador,Agente")]
        public IActionResult Edit(int id)
        {
            var alumno = repoAlumno.ObtenerPorId(id);
            if (alumno == null)
            {
                return NotFound();
            }
            CargarEscuelasViewBag();
            return View(alumno);
        }

        // POST: Alumno/Edit/5
        [Authorize(Roles = "Administrador, Agente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Alumno alumno)
        {
            if (id != alumno.ID_Alumno)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    repoAlumno.Modificacion(alumno);
                    TempData["MensajeExito"] = "Datos del alumno modificados correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                CargarEscuelasViewBag();
                return View(alumno);
            }
            catch
            {
                CargarEscuelasViewBag();
                return View(alumno);
            }
        }

        // GET: Alumno/Delete/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var alumno = repoAlumno.ObtenerPorId(id);
            if (alumno == null)
            {
                return NotFound();
            }
            return View(alumno);
        }

        // POST: Alumno/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repoAlumno.Baja(id);
                TempData["MensajeExito"] = "Alumno eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FOREIGN KEY"))
                {
                    TempData["MensajeError"] = "No se puede eliminar el alumno, tiene registros de vacunación asociados.";
                } else {
                    TempData["MensajeError"] = "Error al eliminar el alumno.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}