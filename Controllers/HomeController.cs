using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MediScope.Models;

namespace MediScope.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
        Console.WriteLine("Hello World!");
    }

    public IActionResult Index()
    {
        return View();
    }
}