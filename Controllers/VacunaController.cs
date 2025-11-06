using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo el Admin puede gestionar vacunas
    public class VacunaController : Controller
    {
        private readonly IRepositorioVacuna repoVacuna;

        public VacunaController(IRepositorioVacuna repoVacuna)
        {
            this.repoVacuna = repoVacuna;
        }

        // GET: Vacuna
        public IActionResult Index()
        {
            var lista = repoVacuna.ObtenerTodos();
            return View(lista);
        }

        // GET: Vacuna/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vacuna/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vacuna vacuna)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repoVacuna.Alta(vacuna);
                    TempData["MensajeExito"] = "Vacuna creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                return View(vacuna);
            }
            catch
            {
                return View(vacuna);
            }
        }

        // GET: Vacuna/Edit/5
        public IActionResult Edit(int id)
        {
            var vacuna = repoVacuna.ObtenerPorId(id);
            if (vacuna == null)
            {
                return NotFound();
            }
            return View(vacuna);
        }

        // POST: Vacuna/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Vacuna vacuna)
        {
            if (id != vacuna.ID_Vacuna)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    repoVacuna.Modificacion(vacuna);
                    TempData["MensajeExito"] = "Vacuna modificada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                return View(vacuna);
            }
            catch
            {
                return View(vacuna);
            }
        }

        // GET: Vacuna/Delete/5
        public IActionResult Delete(int id)
        {
            var vacuna = repoVacuna.ObtenerPorId(id);
            if (vacuna == null)
            {
                return NotFound();
            }
            return View(vacuna);
        }

        // POST: Vacuna/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repoVacuna.Baja(id);
                TempData["MensajeExito"] = "Vacuna eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FOREIGN KEY"))
                {
                    TempData["MensajeError"] = "No se puede eliminar la vacuna, está siendo utilizada en registros.";
                } else {
                    TempData["MensajeError"] = "Error al eliminar la vacuna.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}