using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo el Admin puede gestionar agentes
    public class AgenteSanitarioController : Controller
    {
        private readonly IRepositorioAgenteSanitario repoAgente;
        private readonly IRepositorioUsuario repoUsuario;

        public AgenteSanitarioController(IRepositorioAgenteSanitario repoAgente, IRepositorioUsuario repoUsuario)
        {
            this.repoAgente = repoAgente;
            this.repoUsuario = repoUsuario;
        }

        // GET: AgenteSanitario
        public IActionResult Index()
        {
            var lista = repoAgente.ObtenerTodos();
            return View(lista);
        }

        // GET: AgenteSanitario/Create
        public IActionResult Create()
        {
            // La vista de Create necesitará campos para Agente y para Usuario (Email, Password, Rol)
            return View();
        }

        // POST: AgenteSanitario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AgenteSanitario agente, string email, string password, string rol)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 1. Crear el Usuario primero
                    var nuevoUsuario = new Usuario
                    {
                        Email = email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                        Rol = rol,
                        AvatarURL = "/uploads/avatars/default.png" // Avatar por defecto
                    };
                    repoUsuario.Alta(nuevoUsuario); // El ID_Usuario se carga en el objeto

                    // 2. Asignar el nuevo ID de Usuario al Agente
                    agente.ID_Usuario = nuevoUsuario.ID_Usuario;

                    // 3. Crear el Agente
                    repoAgente.Alta(agente);

                    TempData["MensajeExito"] = "Agente creado con éxito (y su cuenta de usuario).";
                    return RedirectToAction(nameof(Index));
                }
                return View(agente);
            }
            catch(Exception ex)
            {
                ViewData["MensajeError"] = $"Error: {ex.Message}";
                // (Si falla, idealmente deberíamos borrar el usuario que se creó)
                return View(agente);
            }
        }

        // GET: AgenteSanitario/Edit/5
        public IActionResult Edit(int id)
        {
            var agente = repoAgente.ObtenerPorId(id);
            if (agente == null)
            {
                return NotFound();
            }
            // El modelo AgenteSanitario ya carga su Usuario (ver Repositorio)
            return View(agente);
        }

        // POST: AgenteSanitario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AgenteSanitario agente)
        {
            if (id != agente.ID_Agente)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Nota: Este Edit no edita los datos del Usuario (email, pass, rol)
                    // Solo edita los datos del Agente (nombre, dni, etc.)
                    // Para editar el usuario, necesitarías campos extras y lógica.
                    repoAgente.Modificacion(agente);
                    TempData["MensajeExito"] = "Agente modificado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                return View(agente);
            }
            catch
            {
                return View(agente);
            }
        }

        // GET: AgenteSanitario/Delete/5
        public IActionResult Delete(int id)
        {
             var agente = repoAgente.ObtenerPorId(id);
            if (agente == null)
            {
                return NotFound();
            }
            return View(agente);
        }

        // POST: AgenteSanitario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Para borrar un agente, idealmente también borramos su usuario
                var agente = repoAgente.ObtenerPorId(id);
                if (agente != null)
                {
                    repoAgente.Baja(id); // Borra agente

                    if (agente.ID_Usuario.HasValue)
                    {
                        //repoUsuario.Baja(agente.ID_Usuario.Value); // Borra usuario
                        // Nota: IRepositorioUsuario no tiene "Baja". Deberías agregarla.
                    }
                }
                
                TempData["MensajeExito"] = "Agente eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                 if (ex.Message.Contains("FOREIGN KEY"))
                {
                    TempData["MensajeError"] = "No se puede eliminar el agente, tiene registros de vacunación asociados.";
                } else {
                    TempData["MensajeError"] = "Error al eliminar el agente.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}