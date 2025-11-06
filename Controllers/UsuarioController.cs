using Microsoft.AspNetCore.Mvc;
using VacunacionTPFinal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting; 
using System.IO;

namespace VacunacionTPFinal.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IRepositorioUsuario repoUsuario;
        private readonly IRepositorioAgenteSanitario repoAgente;
        private readonly IWebHostEnvironment environment; 

        public UsuariosController(IRepositorioUsuario repoUsuario, IRepositorioAgenteSanitario repoAgente, IWebHostEnvironment environment)
        {
            this.repoUsuario = repoUsuario;
            this.repoAgente = repoAgente;
            this.environment = environment;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var usuario = repoUsuario.ObtenerPorEmail(email);

                if (usuario == null)
                {
                    ViewData["Mensaje"] = "Datos incorrectos. Intente de nuevo.";
                    return View();
                }

                bool esPasswordValida = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);

                if (!esPasswordValida)
                {
                    ViewData["Mensaje"] = "Datos incorrectos. Intente de nuevo.";
                    return View();
                }

                var agente = repoAgente.ObtenerPorUsuarioId(usuario.ID_Usuario);
                string nombreCompleto = agente != null ? $"{agente.Nombre} {agente.Apellido}" : usuario.Email;
                string avatar = !string.IsNullOrEmpty(usuario.AvatarURL) ? usuario.AvatarURL : "/uploads/avatars/default.png";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, nombreCompleto),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.NameIdentifier, usuario.ID_Usuario.ToString()),
                    new Claim("AvatarURL", avatar),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                };

                if (agente != null)
                {
                    claims.Add(new Claim("AgenteId", agente.ID_Agente.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = $"Error al iniciar sesión: {ex.Message}";
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuarios");
        }

        // Muestra el perfil del usuario (con formulario para cambiar avatar)
        [Authorize]
        public IActionResult Perfil()
        {
            ViewData["Nombre"] = User.Identity.Name;
            ViewData["Email"] = User.FindFirst(ClaimTypes.Email)?.Value;
            ViewData["Avatar"] = User.FindFirst("AvatarURL")?.Value;
            ViewData["Rol"] = User.FindFirst(ClaimTypes.Role)?.Value;

            return View(); 
        }

        // Acción POST para manejar la subida del avatar
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CambiarAvatar(IFormFile avatar)
        {
            try
            {
                if (avatar != null && avatar.Length > 0)
                {
                    // 1. Obtener el ID del usuario logueado
                    var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    // 2. Definir la ruta donde se guardará
                    // environment.WebRootPath apunta a la carpeta 'wwwroot'
                    string wwwRootPath = environment.WebRootPath;
                    string uploadsDir = Path.Combine(wwwRootPath, "uploads", "avatars");
                    
                    // Asegurarse que el directorio exista
                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    // 3. Crear un nombre único para el archivo
                    string extension = Path.GetExtension(avatar.FileName);
                    string nombreArchivo = $"avatar_{idUsuario}{extension}";
                    string filePath = Path.Combine(uploadsDir, nombreArchivo);

                    // 4. Guardar el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(fileStream);
                    }

                    // 5. Guardar la ruta en la base de datos
                    string urlParaDB = $"/uploads/avatars/{nombreArchivo}";
                    repoUsuario.ModificarAvatar(idUsuario, urlParaDB);

                    // 6. Actualizar la cookie de sesión (Claim) con la nueva URL
                    // Para esto, debe desloguear y loguear "silenciosamente"
                    
                    var user = (ClaimsPrincipal)User.Clone(); 
                    var identity = (ClaimsIdentity)user.Identity;
                    
                    var avatarClaim = identity.FindFirst("AvatarURL");
                    if(avatarClaim != null)
                    {
                        identity.RemoveClaim(avatarClaim);
                    }
                    identity.AddClaim(new Claim("AvatarURL", urlParaDB));

                    // Re-autenticar al usuario con la info actualizada
                    await HttpContext.SignOutAsync();
                    await HttpContext.SignInAsync(user);

                    TempData["MensajeExito"] = "Avatar actualizado correctamente.";
                }
                else
                {
                    TempData["MensajeError"] = "No se seleccionó ningún archivo.";
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al subir el avatar: {ex.Message}";
            }

            return RedirectToAction("Perfil");
        }


        // --- Registro (Ejemplo de cómo hashear la contraseña) ---
        // (Descomenta esto y crea la vista Views/Usuarios/Registro.cshtml para habilitarlo)
        /*
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registro(Usuario usuario, string password)
        {
            if (password == null)
            {
                ViewData["Mensaje"] = "La contraseña es obligatoria.";
                return View(usuario);
            }

            try
            {
                // Hasheamos la contraseña
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                usuario.PasswordHash = passwordHash;

                // Asignamos un rol por defecto
                usuario.Rol = "Agente"; // O "Administrador" si es un panel de setup
                usuario.AvatarURL = "/uploads/avatars/default.png"; // Avatar por defecto

                repoUsuario.Alta(usuario);
                
                TempData["MensajeExito"] = "Usuario registrado con éxito. Ya puede iniciar sesión.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Capturar error de DNI/Email duplicado (MySQL devuelve una excepción)
                if (ex.Message.Contains("Duplicate entry"))
                {
                     ViewData["Mensaje"] = "El Email o DNI ya está registrado.";
                } else {
                     ViewData["Mensaje"] = $"Error al registrar: {ex.Message}";
                }
                return View(usuario);
            }
        }
        */
    }
}