using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;

public class AccountController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}