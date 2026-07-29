using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;
using System.Threading.Tasks;

public class AuthController : Controller
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;

    public AuthController(IRepositorioUsuario repositorioUsuario, IAuthService authService, IWebHostEnvironment env)
    {
        _repositorioUsuario = repositorioUsuario;
        _authService = authService;
        _env = env;
    }

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        var usuario = _repositorioUsuario.ObtenerPorEmail(email);

        if (usuario == null || !_authService.VerifyPassword(password, usuario.PasswordHash))
        {
            ViewBag.Error = "Email o contraseña incorrectos.";
            return View();
        }

        var claimsPrincipal = _authService.CreateClaimsPrincipal(usuario);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }

    // GET: /Auth/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View(new Usuario());
    }

    // POST: /Auth/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Usuario model, string password)
    {
        if (string.IsNullOrWhiteSpace(model.Rol)) model.Rol = "Agente";

        ModelState.Remove(nameof(model.PasswordHash));
        ModelState.Remove(nameof(model.Rol));

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("password", "La contraseña es obligatoria.");
        }

        if (model.AvatarFile == null || model.AvatarFile.Length == 0)
        {
            ModelState.AddModelError(nameof(model.AvatarFile), "La foto de perfil (avatar) es obligatoria.");
        }

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var exists = _repositorioUsuario.ObtenerPorEmail(model.Email);
            if (exists != null)
            {
                ModelState.AddModelError("", "Ya existe un usuario registrado con este correo electrónico.");
            }
        }

        if (!ModelState.IsValid) return View(model);

        try
        {
            // Guardar avatar si viene
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.AvatarFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(stream);
                }
                model.AvatarURL = $"/uploads/avatars/{fileName}";
            }

            // Hash y guardar
            model.PasswordHash = _authService.HashPassword(password);

            var newId = _repositorioUsuario.Alta(model);
            if (newId <= 0)
            {
                ModelState.AddModelError("", "No se pudo registrar el usuario. Verifique los datos.");
                return View(model);
            }

            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("DNI", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("Duplicate", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Ya existe un usuario registrado con ese DNI o datos duplicados.");
            }
            else
            {
                ModelState.AddModelError("", "Error al registrar usuario: " + ex.Message);
            }
            return View(model);
        }
    }
}