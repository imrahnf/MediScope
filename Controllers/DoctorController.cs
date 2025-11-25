using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;

public class DoctorController : Controller
{
    // GET
    public IActionResult Index()
    
    {
        return View();
    }
}