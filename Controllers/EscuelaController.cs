using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; // Para IFormFile
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Controllers
{
     
    public class EscuelaController : Controller
    {
        private readonly IRepositorioEscuela repoEscuela;
        private readonly IWebHostEnvironment environment; 

        public EscuelaController(IRepositorioEscuela repoEscuela, IWebHostEnvironment environment)
        {
            this.repoEscuela = repoEscuela;
            this.environment = environment;
        }

        // GET: Escuela
        public IActionResult Index()
        {
            var lista = repoEscuela.ObtenerTodos();
            return View(lista);
        }

        // GET: Escuela/Details/5
        public IActionResult Details(int id)
        {
            var escuela = repoEscuela.ObtenerPorId(id);
            if (escuela == null)
            {
                return NotFound();
            }
            return View(escuela);
        }

        // GET: Escuela/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Escuela/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Escuela escuela, IFormFile? foto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (foto != null && foto.Length > 0)
                    {
                        string wwwRootPath = environment.WebRootPath;
                        string uploadsDir = Path.Combine(wwwRootPath, "uploads", "escuelas");
                        if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
                        
                        string extension = Path.GetExtension(foto.FileName);
                        string nombreArchivo = $"escuela_{Guid.NewGuid()}{extension}";
                        string filePath = Path.Combine(uploadsDir, nombreArchivo);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await foto.CopyToAsync(fileStream);
                        }
                        escuela.Fotos = $"/uploads/escuelas/{nombreArchivo}"; // Guardamos la ruta
                    }

                    repoEscuela.Alta(escuela);
                    TempData["MensajeExito"] = "Escuela creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                return View(escuela);
            }
            catch
            {
                return View(escuela);
            }
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var escuela = repoEscuela.ObtenerPorId(id);
            if (escuela == null)
            {
                return NotFound();
            }
            return View(escuela);
        }

        // POST: Escuela/Edit/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Escuela escuela, IFormFile? foto)
        {
            if (id != escuela.ID_Escuela)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (foto != null && foto.Length > 0)
                    {
                                               
                        string wwwRootPath = environment.WebRootPath;
                        string uploadsDir = Path.Combine(wwwRootPath, "uploads", "escuelas");
                        if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
                        
                        string extension = Path.GetExtension(foto.FileName);
                        string nombreArchivo = $"escuela_{escuela.ID_Escuela}{extension}";
                        string filePath = Path.Combine(uploadsDir, nombreArchivo);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await foto.CopyToAsync(fileStream);
                        }
                        escuela.Fotos = $"/uploads/escuelas/{nombreArchivo}";
                    }
                    
                    repoEscuela.Modificacion(escuela);
                    TempData["MensajeExito"] = "Escuela modificada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                return View(escuela);
            }
            catch
            {
                return View(escuela);
            }
        }

        // GET: Escuela/Delete/5
        public IActionResult Delete(int id)
        {
            var escuela = repoEscuela.ObtenerPorId(id);
            if (escuela == null)
            {
                return NotFound();
            }
            return View(escuela);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repoEscuela.Baja(id);
                TempData["MensajeExito"] = "Escuela eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                
                if (ex.Message.Contains("FOREIGN KEY"))
                {
                     TempData["MensajeError"] = "No se puede eliminar la escuela porque tiene alumnos asociados.";
                } else {
                     TempData["MensajeError"] = "Error al eliminar la escuela.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}