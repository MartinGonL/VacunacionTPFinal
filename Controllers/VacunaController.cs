using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class VacunaController : Controller
{
    private readonly IRepositorioVacuna _repoVacuna;

    public VacunaController(IRepositorioVacuna repoVacuna)
    {
        _repoVacuna = repoVacuna;
    }

    // GET: /Vacuna
    public IActionResult Index()
    {
        return View(_repoVacuna.ObtenerTodos());
    }

    // GET: /Vacuna/Create
    [Authorize(Roles = "Administrador")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Vacuna/Create
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public IActionResult Create(Vacuna vacuna)
    {
        try
        {
            _repoVacuna.Alta(vacuna);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(vacuna);
        }
    }

    // GET: /Vacuna/Edit/5
    [Authorize(Roles = "Administrador")]
    public IActionResult Edit(int id)
    {
        var vacuna = _repoVacuna.ObtenerPorId(id);
        if (vacuna == null) return NotFound();
        return View(vacuna);
    }

    // POST: /Vacuna/Edit/5
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public IActionResult Edit(int id, Vacuna vacuna)
    {
        try
        {
            vacuna.VacunaID = id;
            _repoVacuna.Modificar(vacuna);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(vacuna);
        }
    }

    // GET: /Vacuna/Delete/5
    [Authorize(Roles = "Administrador")]
    public IActionResult Delete(int id)
    {
        var vacuna = _repoVacuna.ObtenerPorId(id);
        if (vacuna == null) return NotFound();
        return View(vacuna);
    }

    // POST: /Vacuna/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrador")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repoVacuna.Baja(id);
        return RedirectToAction(nameof(Index));
    }
}