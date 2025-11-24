using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;

public class AppointmentController : Controller
{
    
    [Authorize(Roles = "Patient")]
    public IActionResult Book()
    {
        return View();
    }
}