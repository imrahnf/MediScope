using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;

public class PatientController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}