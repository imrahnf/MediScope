using MediScope.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;


public class AccountController : Controller
{
    private readonly AuthenticationService _authenticationService;
    public AccountController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    // Sign In
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _authenticationService.ValidateUser(username, password);
        if (user == null)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }
        
        // Session varaibles
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Role", user.Role);        
        
        // redirect based on role
        return user.Role switch
        {
            // TODO: add routes for other roles (admin, doctor)
            "Patient" => RedirectToAction("Index", "Patient"),
            _ => RedirectToAction("Login", "Account")
        };
    }
    
    // Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    
    // Base Login Page
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}