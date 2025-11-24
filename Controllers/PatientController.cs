using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;

[Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}