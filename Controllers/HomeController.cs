using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VacunacionTPFinal.Models; // Necesario para ErrorViewModel

namespace VacunacionTPFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        // Página pública
        public IActionResult Privacy()
        {
            return View();
        }

        // Página para usuarios que no tienen el rol necesario
        [Authorize]
        public IActionResult AccesoDenegado()
        {
            ViewData["Nombre"] = User.Identity.Name;
            ViewData["Rol"] = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

  
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}